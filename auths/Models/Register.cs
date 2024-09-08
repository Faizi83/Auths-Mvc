using System.ComponentModel.DataAnnotations;

namespace auths.Models
{
    public class Register
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
       
        public string Name { get; set; }

        [Required(ErrorMessage = "Age is required.")]
        [Range(1, 120, ErrorMessage = "Age must be between 1 and 120.")]
      
        public int Age { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
     
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
      
        public string Password { get; set; }

      
    }
}