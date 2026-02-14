using System.ComponentModel.DataAnnotations;

namespace Web.API.Dtos.Account
{
    public class RegisterDto
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
