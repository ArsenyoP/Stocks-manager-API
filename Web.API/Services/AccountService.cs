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
        public AccountService(UserManager<AppUser> userManager, ITokenService tokenService,
            SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
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
                    throw new IdentityException($"Error while adding roles: {errors}");
                }
            }
            else
            {
                var errors = string.Join("; ", createdUser.Errors.Select(s => s.Description));
                throw new IdentityException($"Error while creating user: {errors}");
            }
        }

        public async Task<NewUserDto> LoginUser(LoginDto loginDto, CancellationToken ct)
        {
            var user = await _userManager.FindByNameAsync(loginDto.UserName);

            if (user == null) throw new IdentityException("Can't find user");

            var passwordCheckResylt = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!passwordCheckResylt.Succeeded) throw new IdentityException("Username or password incorect");

            return new NewUserDto
            {
                UserName = user.UserName,
                Email = user.Email,
                Token = _tokenService.CreateToken(user)
            };
        }
    }
}
