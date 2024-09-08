using Microsoft.AspNetCore.Identity;

namespace auths.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}
