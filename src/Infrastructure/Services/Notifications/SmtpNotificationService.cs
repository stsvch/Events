using Events.Application.Interfaces;
using Events.Infrastructure.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Events.Infrastructure.Services.Notifications
{
    public class SmtpNotificationService : INotificationService
    {
        private readonly SmtpSettings _s;
        private readonly ILogger<SmtpNotificationService> _logger;

        public SmtpNotificationService(
            IOptions<SmtpSettings> opts,
            ILogger<SmtpNotificationService> logger)
        {
            _s = opts.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string htmlBody, CancellationToken ct = default)
        {
            var msg = new MimeMessage();
            msg.From.Add(new MailboxAddress("Events App", _s.Username));
            msg.To.Add(MailboxAddress.Parse(to));
            msg.Subject = subject;
            msg.Body = new TextPart("html") { Text = htmlBody };

            using var client = new SmtpClient { Timeout = 10_000 };
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(TimeSpan.FromSeconds(15));

            // Пробуем автоматический режим: MailKit сам выберет SSL или STARTTLS
            _logger.LogInformation("SMTP: Connecting to {Host}:{Port} with SecureSocketOptions.Auto", _s.Host, _s.Port);
            try
            {
                await client.ConnectAsync(_s.Host, _s.Port, SecureSocketOptions.Auto, cts.Token);
                _logger.LogInformation("SMTP: Connected (Auto)");
            }
            catch (Exception exAuto)
            {
                _logger.LogWarning(exAuto, "SMTP: Auto connect failed, trying explicit StartTls on port 587");
                // Явно STARTTLS на 587
                await client.ConnectAsync(_s.Host, 587, SecureSocketOptions.StartTls, cts.Token);
                _logger.LogInformation("SMTP: Connected (StartTls)");
            }

            // Гугл по умолчанию предлагает XOAUTH2 — переключимся на plain LOGIN/PLAIN
            client.AuthenticationMechanisms.Remove("XOAUTH2");

            _logger.LogInformation("SMTP: Authenticating as {Username}", _s.Username);
            await client.AuthenticateAsync(_s.Username, _s.Password, cts.Token);
            _logger.LogInformation("SMTP: Authenticated");

            await client.SendAsync(msg, cts.Token);
            _logger.LogInformation("SMTP: Email sent to {To}", to);

            await client.DisconnectAsync(true, cts.Token);
            _logger.LogInformation("SMTP: Disconnected");
        }
    }
}
