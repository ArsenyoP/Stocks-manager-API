using System.ComponentModel.DataAnnotations;

namespace Web.API.Dtos.Account
{
    public class LoginDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }

    }
}
