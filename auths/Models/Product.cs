using System.ComponentModel.DataAnnotations;

namespace auths.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "The product name is required.")]
        [StringLength(100, ErrorMessage = "The product name cannot be longer than 100 characters.")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "The price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "The price must be greater than zero.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "The Description is required.")]
        [StringLength(500, ErrorMessage = "The description cannot be longer than 500 characters.")]
        public string Description { get; set; }


        public string RegisteredId { get; set; }


    }
}
