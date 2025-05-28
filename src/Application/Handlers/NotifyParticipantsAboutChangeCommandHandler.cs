using Events.Application.Commands;
using Events.Application.Interfaces;
using Events.Domain.Exceptions;
using Events.Domain.Repositories;
using MediatR;

namespace Events.Application.Handlers
{
    public class NotifyParticipantsAboutChangeCommandHandler : IRequestHandler<NotifyParticipantsAboutChangeCommand, Unit>
    {
        private readonly IEventRepository _eventRepo;
        private readonly INotificationService _notifier;

        public NotifyParticipantsAboutChangeCommandHandler( IEventRepository eventRepo,INotificationService notifier)
        {
            _eventRepo = eventRepo;
            _notifier = notifier;
        }

        public async Task<Unit> Handle(NotifyParticipantsAboutChangeCommand command, CancellationToken cancellationToken)
        {
            var evt = await _eventRepo.GetByIdWithDetailsAsync(command.EventId, cancellationToken);
            if (evt == null)
            {
                throw new EntityNotFoundException(command.EventId);
            }

            if (!evt.Participants.Any())
            {
                return Unit.Value;
            }

            string subject = $"Update on event “{evt.Title}”";
            string body = $@"
            <p>Hello,</p>
            <p>The event <strong>{evt.Title}</strong> has been updated:</p>
            <p>{command.Message}</p>
            <p>Please review the changes in your dashboard.</p>
            <hr/>
            <p>Best regards,<br/>Events App</p> ";

            foreach (var ep in evt.Participants)
            {
                var email = ep.Participant.Email.Value;
                try
                {
                    await _notifier.SendEmailAsync(email, subject, body, cancellationToken);
                }
                catch (Exception ex)
                {
                }
            }

            return Unit.Value;
        }
    }

}
