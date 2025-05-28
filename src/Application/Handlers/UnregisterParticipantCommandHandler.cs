using Events.Application.Commands;
using Events.Domain.Exceptions;
using Events.Domain.Repositories;
using MediatR;


namespace Events.Application.Handlers
{
    public class UnregisterParticipantCommandHandler : IRequestHandler<UnregisterParticipantCommand, Unit>
    {
        private readonly IEventRepository _eventRepo;
        private readonly IParticipantRepository _partRepo;

        public UnregisterParticipantCommandHandler(IEventRepository eventRepo, IParticipantRepository partRepo)
        {
            _eventRepo = eventRepo;
            _partRepo = partRepo;
        }

        public async Task<Unit> Handle( UnregisterParticipantCommand command,CancellationToken cancellationToken)
        {
            var evt = await _eventRepo
                .GetByIdAsync(command.EventId, cancellationToken)
                ?? throw new EntityNotFoundException(command.EventId);

            var participant = await _partRepo
                .GetByIdAsync(command.ParticipantId, cancellationToken)
                ?? throw new EntityNotFoundException(command.ParticipantId);

            if (participant.UserId != command.UserId)
                throw new ForbiddenException("You can only cancel your own registration.");

            evt.RemoveParticipant(participant.Id);
            await _eventRepo.UpdateAsync(evt, cancellationToken);

            return Unit.Value;
        }
    }
}
