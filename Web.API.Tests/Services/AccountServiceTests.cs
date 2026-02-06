using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.API.Dtos.Account;
using Web.API.Exceptions;
using Web.API.Interfaces;
using Web.API.Models;
using Web.API.Services;

namespace Web.API.Tests.Services
{
    public class AccountServiceTests
    {
        private readonly Mock<UserManager<AppUser>> _userManagerMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<SignInManager<AppUser>> _signInManagerMock;
        private readonly Mock<ILogger<AccountService>> _loggerMock;
        private readonly AccountService _accountService;
        public AccountServiceTests()
        {
            var userStoreMock = new Mock<IUserStore<AppUser>>();
            _userManagerMock = new Mock<UserManager<AppUser>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);

            _signInManagerMock = new Mock<SignInManager<AppUser>>(
                _userManagerMock.Object,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<AppUser>>().Object,
                null, null, null, null);

            _tokenServiceMock = new Mock<ITokenService>();
            _loggerMock = new Mock<ILogger<AccountService>>();

            _accountService = new AccountService(_userManagerMock.Object, _tokenServiceMock.Object,
                _signInManagerMock.Object, _loggerMock.Object);
        }


        [Fact]
        public async Task CreateNewUser_EverythingRight_ReturnsNewUserDto()
        {
            var registerDto = new RegisterDto
            {
                Email = "test@gmail.com",
                UserName = "TestUsername",
                Password = "1234"
            };


            var successIdentityResult = IdentityResult.Success;

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
                .ReturnsAsync(successIdentityResult);

            _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<AppUser>(), "User"))
                .ReturnsAsync(successIdentityResult);

            _tokenServiceMock.Setup(x => x.CreateToken(It.IsAny<AppUser>()))
                .Returns("Test_Token_JWT");


            var result = await _accountService.CreateNewUser(registerDto, new CancellationToken { });


            result.UserName.Should().Be("TestUsername");
            result.Email.Should().Be("test@gmail.com");
            result.Token.Should().Be("Test_Token_JWT");

            _userManagerMock.Verify(x => x.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>()),
                Times.Once, "Shoud call CreateAsync once");
            _userManagerMock.Verify(x => x.AddToRoleAsync(It.IsAny<AppUser>(), "User"),
                Times.Once, "Shoud call AddToRoleAsync once");
            _tokenServiceMock.Verify(x => x.CreateToken(It.IsAny<AppUser>()),
                Times.Once, "Shoud call AddToRoleAsync once");
        }

        [Fact]
        public async Task CreateNewUser_CreateAsyncError_ThrowsIdentityException()
        {
            var registerDto = new RegisterDto
            {
                Email = "test@gmail.com",
                UserName = "TestUsername",
                Password = "1234"
            };

            var identityResult = IdentityResult.Failed();
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
                .ReturnsAsync(identityResult);


            Func<Task> act = async () => await _accountService.CreateNewUser(registerDto, CancellationToken.None);



            await act.Should().ThrowAsync<IdentityException>();

            _userManagerMock.Verify(x => x.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>()),
                Times.Once);

            _userManagerMock.Verify(x => x.AddToRoleAsync(It.IsAny<AppUser>(), It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task LoginUser_EverythingRight_ReturnsNewUserDto()
        {
            var loginDto = new LoginDto
            {
                Password = "TestPassword",
                UserName = "TestUsername"
            };

            var user = new AppUser
            {
                UserName = loginDto.UserName
            };

            _userManagerMock.Setup(x => x.FindByNameAsync(loginDto.UserName))
                .ReturnsAsync(user);

            _signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(user, loginDto.Password, false))
                .ReturnsAsync(SignInResult.Success);

            _tokenServiceMock.Setup(x => x.CreateToken(user))
                .Returns("JWT_TEST_TOKEN");


            var result = await _accountService.LoginUser(loginDto, CancellationToken.None);


            result.UserName.Should().Be("TestUsername");
            result.Token.Should().Be("JWT_TEST_TOKEN");

            _userManagerMock.Verify(x => x.FindByNameAsync(loginDto.UserName),
                Times.Once, "Should call FindByNameAsync once");

            _signInManagerMock.Verify(x => x.CheckPasswordSignInAsync(user, loginDto.Password, false),
                Times.Once, "Should call CheckPasswordSignInAsync once");

            _tokenServiceMock.Verify(x => x.CreateToken(user),
                Times.Once, "Should call CreateToken once");
        }

        [Fact]
        public async Task LoginUser_NullUser_ThrowIdentityException()
        {
            var loginDto = new LoginDto
            {
                UserName = "TestUsername"
            };

            _userManagerMock.Setup(x => x.FindByNameAsync(loginDto.UserName))
                .ReturnsAsync((AppUser?)null);


            Func<Task> act = async () => await _accountService.LoginUser(loginDto, CancellationToken.None);


            await act.Should().ThrowAsync<IdentityException>()
                .WithMessage("Username or password incorrect");

            _signInManagerMock.Verify(x => x.CheckPasswordSignInAsync(It.IsAny<AppUser>(), loginDto.Password, false),
                Times.Never);
        }

        [Fact]
        public async Task LoginUser_IncorectPassword_ThrowIdentityException()
        {
            var loginDto = new LoginDto
            {
                Password = "TestPassword",
                UserName = "TestUsername"
            };

            var user = new AppUser { UserName = loginDto.UserName, Email = "test@test.com" };

            _userManagerMock.Setup(x => x.FindByNameAsync(loginDto.UserName))
                .ReturnsAsync(user);

            _signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(user, loginDto.Password, false))
                .ReturnsAsync(SignInResult.Failed);


            Func<Task> act = async () => await _accountService.LoginUser(loginDto, CancellationToken.None);


            await act.Should().ThrowAsync<IdentityException>()
                .WithMessage("Username or password incorrect");

            _userManagerMock.Verify(x => x.FindByNameAsync(loginDto.UserName),
                Times.Once);

            _signInManagerMock.Verify(x => x.CheckPasswordSignInAsync(It.IsAny<AppUser>(), loginDto.Password, false),
                Times.Once);
        }
    }
}
