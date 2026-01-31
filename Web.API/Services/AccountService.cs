using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web.API.Dtos.Account;
using Web.API.Exceptions;
using Web.API.Interfaces;
using Web.API.Interfaces.IServices;
using Web.API.Models;

namespace Web.API.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<AccountService> _logger;

        public AccountService(UserManager<AppUser> userManager, ITokenService tokenService,
            SignInManager<AppUser> signInManager, ILogger<AccountService> logger)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<NewUserDto> CreateNewUser(RegisterDto register, CancellationToken ct)
        {
            var AppUserModel = new AppUser
            {
                UserName = register.UserName,
                Email = register.Email
            };

            var createdUser = await _userManager.CreateAsync(AppUserModel, register.Password);

            if (createdUser.Succeeded)
            {
                var rolesResult = await _userManager.AddToRoleAsync(AppUserModel, "User");
                if (rolesResult.Succeeded)
                {
                    _logger.LogInformation("User: {User} with ID: {UserID} was successfully created",
                        AppUserModel.UserName, AppUserModel.Id);
                    return new NewUserDto
                    {
                        UserName = AppUserModel.UserName,
                        Email = AppUserModel.Email,
                        Token = _tokenService.CreateToken(AppUserModel)
                    };
                }
                else
                {
                    var errors = string.Join("; ", rolesResult.Errors.Select(s => s.Description));
                    _logger.LogError("Error while adding roles for {User} with ID: {UserID} errors: {Errors}",
                        AppUserModel.UserName, AppUserModel.Id, errors);

                    throw new IdentityException($"Error while adding roles: {errors}");
                }
            }
            else
            {
                var errors = string.Join("; ", createdUser.Errors.Select(s => s.Description));
                _logger.LogError("Error while creating user: {User}, with ID: {UserID} errors: {Errors}",
                        AppUserModel.UserName, AppUserModel.Id, errors);
                throw new IdentityException($"Error while creating user: {errors}");
            }
        }

        public async Task<NewUserDto> LoginUser(LoginDto loginDto, CancellationToken ct)
        {
            var user = await _userManager.FindByNameAsync(loginDto.UserName);

            if (user == null)
            {
                _logger.LogWarning("Login failed: User with username {UserName} not found", loginDto.UserName);
                throw new IdentityException("Username or password incorrect");
            }

            var passwordCheckResult = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!passwordCheckResult.Succeeded)
            {
                _logger.LogWarning("Login failed: Incorrect password for user {UserName}", loginDto.UserName);
                throw new IdentityException("Username or password incorrect");
            }

            _logger.LogInformation("User {UserName} (ID: {UserId}) logged in successfully", user.UserName, user.Id);

            return new NewUserDto
            {
                UserName = user.UserName,
                Email = user.Email,
                Token = _tokenService.CreateToken(user)
            };
        }
    }
}
