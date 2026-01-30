using Web.API.Dtos.Account;

namespace Web.API.Interfaces.IServices
{
    public interface IAccountService
    {
        Task<NewUserDto> CreateNewUser(RegisterDto register, CancellationToken ct);
        Task<NewUserDto> LoginUser(LoginDto loginDto, CancellationToken ct);

    }
}
