using Microsoft.AspNetCore.Identity;

namespace Web.API.Models
{
    public class AppUser : IdentityUser
    {
        public List<Portfolio> Portfolios { get; set; } = new List<Portfolio>();
    }
}
