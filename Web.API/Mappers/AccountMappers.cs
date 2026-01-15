using Web.API.Dtos.Account;
using Web.API.Models;

namespace Web.API.Mappers
{
    public static class AccountMappers
    {
        public static AccountDto FromAppUserToAccountDto(this AppUser user)
        {
            return new AccountDto
            {
                UserName = user.UserName,
                Email = user.Email
            };
        }
    }
}
