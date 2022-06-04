using System.ComponentModel.DataAnnotations;

namespace SexShop.Models
{
    public class ItemModel
    {
        [Key]
        public int Id { get; set; }
        public ProductModel ProductModel { get; set; }
        public int Quantity { get; set; }
    }
}
