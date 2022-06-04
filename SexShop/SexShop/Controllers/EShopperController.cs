using Microsoft.AspNetCore.Mvc;
using SexShop.Data;
using SexShop.Models;
using SexShop.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;

namespace SexShop.Controllers
{
    [Route("[controller]")]
    public class EShopperController : Controller
    {
        private readonly EShopperDbContext _db;
        public EShopperController (EShopperDbContext db)
        {
            _db = db;
        }
        [Route("/")]
        public IActionResult Index()
        {
            var cart = SessionHelper.GetObjectFromJson<List<ItemModel>>(HttpContext.Session, "cart");
            if (cart == null)
            {
                ViewBag.count = 0;
            }
            else
            {
                ViewBag.count = cart.Count();
            }
            IEnumerable<ProductModel> objList = _db.Products;
            ViewBag.objList = objList;

            return View(objList);
        }

        [HttpGet]
        [Route("Category/{q}")]
        public IActionResult Category(string? q)
        {
            var cart = SessionHelper.GetObjectFromJson<List<ItemModel>>(HttpContext.Session, "cart");
            if (cart == null)
            {
                ViewBag.count = 0;
            }
            else
            {
                ViewBag.count = cart.Count();
            }
            IEnumerable<ProductModel> categoryFromDb = 
                from obj in _db.Products
                where obj.Category == q
                select obj;

            IEnumerable<ProductModel> objList = _db.Products;
            ViewBag.objList = objList;

            return View(categoryFromDb);
        }
        [Route ("Detail/{id}")]
        public IActionResult Detail(int? id)
        {
            var cart = SessionHelper.GetObjectFromJson<List<ItemModel>>(HttpContext.Session, "cart");
            if (cart == null)
            {
                ViewBag.count = 0;
            }
            else
            {
                ViewBag.count = cart.Count();
            }
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var product = _db.Products.FirstOrDefault(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            IEnumerable<ProductModel> objList = _db.Products;
            ViewBag.objList = objList;

            return View(product);
        }
        [Authorize]
        [Route ("Cart")]
        public IActionResult Cart()
        {
            var cart = SessionHelper.GetObjectFromJson<List<ItemModel>>(HttpContext.Session, "cart");
            ViewBag.cart = cart;
            try
            {
                ViewBag.total = cart.Sum(item => item.ProductModel.Price * item.Quantity);
            }
            catch(ArgumentNullException e)
            {
                ViewBag.total = "0";
            }
            if (cart == null)
            {
                ViewBag.count = 0;
            }
            else
            {
                ViewBag.count = cart.Count();
            }
            IEnumerable<ProductModel> objList = _db.Products;
            ViewBag.objList = objList;

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route ("Cart")]
        public IActionResult Cart(CartModel obj)
        {
            if (ModelState.IsValid)
            {
                _db.Cart.Add(obj);
                var resp = obj.CartJson.Remove(obj.CartJson.Length - 2, 1);
                var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(resp);
                foreach (KeyValuePair<string, string> item in dict)
                {
                    ProductModel product = _db.Products.Find(int.Parse(item.Key));
                    product.Quantity -= int.Parse(item.Value);
                    _db.Products.Update(product);
                }
                _db.SaveChanges();
                return RedirectToAction("Redirect", new {id = obj.Id});
            }
            return View(obj);
        }
        private int isExist(int id)
        {
            List<ItemModel> cart = SessionHelper.GetObjectFromJson<List<ItemModel>>(HttpContext.Session, "cart");
            for (int i = 0; i < cart.Count; i++)
            {
                if (cart[i].ProductModel.Id.Equals(id))
                {
                    return i;
                }
            }
            return -1;
        }
        [Route("Buy/{id}")]
        public IActionResult Buy(int id)
        {
            if (SessionHelper.GetObjectFromJson<List<ItemModel>>(HttpContext.Session, "cart") == null)
            {
                List<ItemModel> cart = new();
                cart.Add(new ItemModel { ProductModel = _db.Products.Find(id), Quantity = 1 });
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            }
            else
            {
                List<ItemModel> cart = SessionHelper.GetObjectFromJson<List<ItemModel>>(HttpContext.Session, "cart");
                int index = isExist(id);
                if (index != -1)
                {
                    cart[index].Quantity++;
                }
                else
                {
                    cart.Add(new ItemModel { ProductModel = _db.Products.Find(id), Quantity = 1 });
                }
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            }
            return RedirectToAction("Cart");
        }
        [Route("Decrease/{id}")]
        public IActionResult Decrease(int id)
        {
            List<ItemModel> cart = SessionHelper.GetObjectFromJson<List<ItemModel>>(HttpContext.Session, "cart");
            int index = isExist(id);
            if (cart[index].Quantity > 1)
            {
                cart[index].Quantity--;
            }
            else
            {
                return RedirectToAction("Remove", new { id = id });
            }
            SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            return RedirectToAction("Cart");
        }
        [Route("Remove/{id}")]
        public IActionResult Remove(int id)
        {
            List<ItemModel> cart = SessionHelper.GetObjectFromJson<List<ItemModel>>(HttpContext.Session, "cart");
            int index = isExist(id);
            try
            {
                cart.RemoveAt(index);
            }
            catch (ArgumentOutOfRangeException e)
            {

            }
            SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            return RedirectToAction("Cart");
        }

        public async Task<IActionResult> Query(string query)
        {
            var cart = SessionHelper.GetObjectFromJson<List<ItemModel>>(HttpContext.Session, "cart");
            if (cart == null)
            {
                ViewBag.count = 0;
            }
            else
            {
                ViewBag.count = cart.Count();
            }
            var products = from p in _db.Products
                           select p;

            if (!String.IsNullOrEmpty(query))
            {
                products = products.Where(p => p.Name.ToLower().Contains(query.ToLower()));
            }
            IEnumerable<ProductModel> objList = _db.Products;
            ViewBag.objList = objList;

            return View(await products.ToListAsync());
        }
        [Route("Redirect/{id}")]
        public IActionResult Redirect(int? id)
        {
            ViewBag.Id = id;
            return View();
        }
    }
}
