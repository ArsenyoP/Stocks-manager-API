using Web.API.Helpers;
using Web.API.Interfaces.IServices;

namespace Web.API.Jobs
{
    public class SendWelcomeEmailJob
    {
        private readonly IEmailService _emailService;

        public SendWelcomeEmailJob(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task ExecuteAsync(string email, string username)
        {
            await _emailService.SendAsync(email, "Ласкаво просимо до StockApp!",
                EmailTemplates.WelcomeEmaiil(username));
        }
    }
}
