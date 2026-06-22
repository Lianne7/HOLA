using Microsoft.AspNetCore.Identity;

namespace HOLA.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int EcoPoints { get; set; } = 0;

        public int TreeCount { get; set; } = 0;
    }
}