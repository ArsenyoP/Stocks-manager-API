using Hangfire;
using Web.API.Dtos.Account;
using Web.API.Helpers;
using Web.API.Interfaces.IServices;
using Web.API.Jobs;
using Web.API.Models;

namespace Web.API.Services.Decorators
{
    public class EmailNotificationAccountService : IAccountService
    {
        private readonly IAccountService _inner;
        private readonly IEmailService _emailService;
        private readonly ILogger<EmailNotificationAccountService> _logger;
        private readonly IBackgroundJobClient _jobClient;

        public EmailNotificationAccountService(IAccountService inner, IEmailService emailService,
            ILogger<EmailNotificationAccountService> logger, IBackgroundJobClient jobClient)
        {
            _inner = inner;
            _emailService = emailService;
            _logger = logger;
            _jobClient = jobClient;
        }

        public async Task<NewUserDto> CreateNewUser(RegisterDto register, CancellationToken ct, string role = "User")
        {
            var userDto = await _inner.CreateNewUser(register, ct, role);
            try
            {
                _jobClient.Enqueue<SendWelcomeEmailJob>(x =>
                     x.ExecuteAsync(register.Email, register.UserName));
                _logger.LogInformation("Background job for sending emails has been executed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send welcome email to {Email}", register.Email);
            }
            return userDto;
        }



        public async Task<AppUser?> GetById(string id)
        {
            return await _inner.GetById(id);
        }

        public async Task<NewUserDto> LoginUser(LoginDto loginDto, CancellationToken ct)
        {
            return await _inner.LoginUser(loginDto, ct);
        }
    }
}
