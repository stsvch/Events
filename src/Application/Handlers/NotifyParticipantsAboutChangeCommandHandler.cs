using Events.Application.Commands;
using Events.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Application.Handlers
{
    public class NotifyParticipantsAboutChangeCommandHandler : IRequestHandler<NotifyParticipantsAboutChangeCommand, Unit>
    {
        private readonly INotificationService _notifier;
        public NotifyParticipantsAboutChangeCommandHandler(INotificationService notifier) => _notifier = notifier;

        public async Task<Unit> Handle(NotifyParticipantsAboutChangeCommand command, CancellationToken cancellationToken)
        {
            await _notifier.NotifyParticipantsAsync(command);
            return Unit.Value;
        }
    }
}
