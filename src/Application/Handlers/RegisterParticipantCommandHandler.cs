using Events.Application.Commands;
using Events.Domain.Exceptions;
using Events.Domain.Repositories;
using Events.Domain.Specifications;
using MediatR;

namespace Events.Application.Handlers
{
    public class RegisterParticipantCommandHandler : IRequestHandler<RegisterParticipantCommand, Unit>
    {
        private readonly IEventRepository _eventRepo;
        private readonly IParticipantRepository _partRepo;

        public RegisterParticipantCommandHandler(IEventRepository eventRepo,IParticipantRepository partRepo)
        {
            _eventRepo = eventRepo;
            _partRepo = partRepo;
        }

        public async Task<Unit> Handle(RegisterParticipantCommand command, CancellationToken cancellationToken)
        {
            var evt = await _eventRepo.GetByIdWithDetailsAsync(command.EventId, cancellationToken);

            var participant = await _partRepo.GetBySpecAsync(
                new ParticipantByUserIdSpecification(command.UserId),
                cancellationToken);

            evt.AddParticipant(participant.Id);
            await _eventRepo.UpdateAsync(evt, cancellationToken);

            return Unit.Value;
        }

    }

}
