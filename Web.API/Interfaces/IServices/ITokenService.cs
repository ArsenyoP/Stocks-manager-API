using Web.API.Models;

namespace Web.API.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);
    }
}
