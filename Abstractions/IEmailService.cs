namespace WebApplication1.Abstractions
{
    public interface IEmailService
    {
        Task SendEmail(string email, string subject, string message, CancellationToken ct = default);
    }
}

