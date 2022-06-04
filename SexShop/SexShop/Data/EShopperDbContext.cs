using Microsoft.EntityFrameworkCore;
using SexShop.Models;

namespace SexShop.Data
{
    public class EShopperDbContext : DbContext
    {
        public EShopperDbContext(DbContextOptions<EShopperDbContext> options)
           : base(options)
        {
        }
        public DbSet <ProductModel> Products { get; set; }
        public DbSet <CartModel> Cart { get; set; }
    }
}
