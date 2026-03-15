using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Web.API.Dtos.Account;
using Web.API.Interfaces;
using Web.API.Interfaces.IServices;
using Web.API.Mappers;

namespace Web.API.Controllers
{
    [EnableRateLimiting("fixed")]
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IAccountService _accountService;

        public AccountController(IAccountRepository accountRepository, IAccountService accountService)
        {
            _accountRepository = accountRepository;
            _accountService = accountService;
        }

        [EnableRateLimiting("auth-limiter")]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto register, CancellationToken ct)
        {
            var userModel = await _accountService.CreateNewUser(register, ct);
            return Ok(userModel);
        }


        [HttpPost("register/admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterDto register, CancellationToken ct)
        {
            var userModel = await _accountService.CreateNewUser(register, ct, "Admin");
            return Ok(userModel);
        }

        [EnableRateLimiting("auth-limiter")]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto, CancellationToken ct)
        {
            var userModel = await _accountService.LoginUser(loginDto, ct);
            return Ok(userModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync(CancellationToken ct)
        {
            var accounts = await _accountRepository.GetAllAsync(ct);
            var accountDtos = accounts.Select(s => s.FromAppUserToAccountDto()).ToList();
            return Ok(accountDtos);
        }
    }
}