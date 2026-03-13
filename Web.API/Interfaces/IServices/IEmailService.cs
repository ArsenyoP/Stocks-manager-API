namespace Web.API.Interfaces.IServices
{
    public interface IEmailService
    {
        Task SendAsync(string toEmail, string subject, string body, CancellationToken ct = default);
    }
}
