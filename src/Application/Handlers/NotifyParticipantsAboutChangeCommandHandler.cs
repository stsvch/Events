using Events.Application.Commands;
using Events.Application.Interfaces;
using Events.Domain.Exceptions;
using Events.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Application.Handlers
{
    public class NotifyParticipantsAboutChangeCommandHandler: IRequestHandler<NotifyParticipantsAboutChangeCommand, Unit>
    {
        private readonly IEventRepository _events;
        private readonly INotificationService _mailer;

        public NotifyParticipantsAboutChangeCommandHandler(
            IEventRepository events,
            INotificationService mailer)
        {
            _events = events;
            _mailer = mailer;
        }

        public async Task<Unit> Handle(
            NotifyParticipantsAboutChangeCommand cmd,
            CancellationToken ct)
        {
            // 1) Загрузить событие с деталями
            var ev = await _events.GetByIdWithDetailsAsync(cmd.EventId, ct)
                     ?? throw new EntityNotFoundException(cmd.EventId);

            // 2) Разослать всем участникам
            var tasks = ev.Participants
                .Select(ep => ep.Participant.Email.Value)
                .Distinct()
                .Select(email => _mailer.SendEmailAsync(
                    email,
                    $"Изменения в событии «{ev.Title}»",
                    cmd.Message));

            await Task.WhenAll(tasks);
            return Unit.Value;
        }
    }

}
