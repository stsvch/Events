using Events.Application.Commands;
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
    public class UnregisterParticipantCommandHandler : IRequestHandler<UnregisterParticipantCommand, Unit>
    {
        private readonly IEventRepository _eventRepo;
        public UnregisterParticipantCommandHandler(IEventRepository eventRepo) => _eventRepo = eventRepo;

        public async Task<Unit> Handle(UnregisterParticipantCommand command, CancellationToken cancellationToken)
        {
            var evt = await _eventRepo.GetByIdAsync(command.EventId, cancellationToken);
            if (evt == null)
                throw new EntityNotFoundException(command.EventId);

            evt.RemoveParticipant(command.ParticipantId);
            await _eventRepo.UpdateAsync(evt, cancellationToken);
            return Unit.Value;
        }
    }
}
