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
                .GetByIdWithDetailsAsync(command.EventId, cancellationToken);

            var participant = await _partRepo
                .GetBySpecAsync(new ParticipantByUserIdSpecification(command.UserId), cancellationToken);


            evt.RemoveParticipant(participant.Id);
            await _eventRepo.UpdateAsync(evt, cancellationToken);

            return Unit.Value;
        }

    }
}
