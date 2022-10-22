using Microsoft.AspNetCore.Identity;

namespace MOneDbContext.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Username { get; set; } = string.Empty;
    }
}
