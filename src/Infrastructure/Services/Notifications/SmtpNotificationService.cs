using Events.Application.Interfaces;
using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using MimeKit;

namespace Events.Infrastructure.Services.Notifications
{
    public class SmtpSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool UseSsl { get; set; }
    }

    public class SmtpNotificationService : INotificationService
    {
        private readonly SmtpSettings _s;
        public SmtpNotificationService(IOptions<SmtpSettings> opts)
            => _s = opts.Value;

        public async Task SendEmailAsync(string to, string subject, string htmlBody, CancellationToken ct = default)
        {
            var msg = new MimeMessage();
            msg.From.Add(new MailboxAddress("Events App", _s.Username));
            msg.To.Add(MailboxAddress.Parse(to));
            msg.Subject = subject;
            var body = new TextPart("html") { Text = htmlBody };
            msg.Body = body;

            using var client = new SmtpClient();
            await client.ConnectAsync(_s.Host, _s.Port, _s.UseSsl, ct);
            await client.AuthenticateAsync(_s.Username, _s.Password, ct);
            await client.SendAsync(msg, ct);
            await client.DisconnectAsync(true, ct);
        }
    }
}