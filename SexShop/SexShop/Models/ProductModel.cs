using System.ComponentModel.DataAnnotations;

namespace SexShop.Models
{
    public class ProductModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public double Price { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public string Image { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public ulong Barcode { get; set; }
    }
}
