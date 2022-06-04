using System.ComponentModel.DataAnnotations;

namespace SexShop.Models
{
    public class CartModel
    {
        [Key]
        public int Id { get; set; }
        public string CartJson { get; set; }
        public string Username { get; set; }
    }
}
