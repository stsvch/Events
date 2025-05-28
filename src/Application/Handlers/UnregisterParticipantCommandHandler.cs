using Events.Application.Commands;
using Events.Domain.Exceptions;
using Events.Domain.Repositories;
using Events.Domain.Specifications;
using MediatR;


namespace Events.Application.Handlers
{
    public class UnregisterParticipantCommandHandler
        : IRequestHandler<UnregisterParticipantCommand, Unit>
    {
        private readonly IEventRepository _eventRepo;
        private readonly IParticipantRepository _partRepo;

        public UnregisterParticipantCommandHandler(
            IEventRepository eventRepo,
            IParticipantRepository partRepo)
        {
            _eventRepo = eventRepo;
            _partRepo = partRepo;
        }

        public async Task<Unit> Handle(UnregisterParticipantCommand command, CancellationToken cancellationToken)
        {
            var evt = await _eventRepo
                .GetByIdWithDetailsAsync(command.EventId, cancellationToken)
                ?? throw new EntityNotFoundException(command.EventId);

            var participant = await _partRepo
                .GetBySpecAsync(new ParticipantByUserIdSpecification(command.UserId), cancellationToken)
                ?? throw new EntityNotFoundException();

            var ep = evt.Participants.SingleOrDefault(x => x.ParticipantId == participant.Id);
            if (ep == null)
                throw new ForbiddenException("You are not registered for this event.");

            evt.RemoveParticipant(participant.Id);
            await _eventRepo.UpdateAsync(evt, cancellationToken);

            return Unit.Value;
        }

    }
}
