

namespace Events.Application.Interfaces
{
    public interface INotificationService
    {
        Task SendEmailAsync(string to, string subject, string htmlBody, CancellationToken ct = default);
    }
}
