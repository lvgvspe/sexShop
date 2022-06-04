using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SexShop.Data;
using SexShop.Models;
using System.Diagnostics;
using System.Text.Json;

namespace SexShop.Controllers
{
    public class AdminController : Controller
    {
        private readonly EShopperDbContext _db;

        public AdminController(EShopperDbContext db)
        {
            _db = db;
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            IEnumerable<ProductModel> objList = _db.Products;
            ViewBag.objList = objList;
            return View(objList);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(ProductModel obj)
        {
            if (ModelState.IsValid)
            {
                _db.Products.Add(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(obj);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var productFromDb = _db.Products.Find(id);

            if (productFromDb == null)
            {
                return NotFound();
            }

            return View(productFromDb);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(ProductModel obj)
        {
            if (ModelState.IsValid)
            {
                _db.Products.Update(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(obj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int? id)
        {
            var obj = _db.Products.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            _db.Products.Remove(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Order(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var order = _db.Cart.Find(id);

            if (order == null)
            {
                return NotFound();
            }
            var resp = order.CartJson.Remove(order.CartJson.Length - 2, 1);
            var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(resp);

            ViewBag.order = dict;
            ViewBag.products = _db.Products;

            double sum = 0;
            foreach(KeyValuePair<string, string> item in dict)
            {
                sum += double.Parse(item.Value) * _db.Products.Find(int.Parse(item.Key)).Price;
            }
            ViewBag.total = sum;

            return View(order);
        }
    }
}