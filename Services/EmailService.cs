using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using WebApplication1.Abstractions;
using WebApplication1.Models.Options;

namespace WebApplication1.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailOptions _options;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptionsMonitor<EmailOptions> options, ILogger<EmailService> logger)
        {
            _options = options.CurrentValue;
            _logger = logger;
        }

        public async Task SendEmail(string email, string subject, string message, CancellationToken ct = default)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("AuthApp", _options.SenderEmail));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = message };

            using (var client = new SmtpClient())
            {
                client.Timeout = 30000;
                _logger.LogInformation("Connecting to SMTP server: {Server}:{Port}", _options.SmtpServer, _options.SmtpPort);
                await client.ConnectAsync(_options.SmtpServer, _options.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls, ct);
                _logger.LogInformation("Connected to SMTP server.");
                await client.AuthenticateAsync(_options.Login, _options.SenderPassword, ct);
                await client.SendAsync(emailMessage, ct);
                await client.DisconnectAsync(true, ct);
            }
        }
    }
}

