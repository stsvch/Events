using Events.Application.Commands;
using Events.Domain.Entities;
using Events.Domain.Exceptions;
using Events.Domain.Repositories;
using Events.Domain.Specifications;
using Events.Domain.ValueObjects;
using MediatR;

namespace Events.Application.Handlers
{
    public class RegisterParticipantCommandHandler : IRequestHandler<RegisterParticipantCommand, Unit>
    {
        private readonly IEventRepository _eventRepo;
        private readonly IParticipantRepository _partRepo;

        public RegisterParticipantCommandHandler(
            IEventRepository eventRepo,
            IParticipantRepository partRepo)
        {
            _eventRepo = eventRepo;
            _partRepo = partRepo;
        }

        public async Task<Unit> Handle(
            RegisterParticipantCommand command,
            CancellationToken cancellationToken)
        {
            var evt = await _eventRepo
                .GetByIdWithDetailsAsync(command.EventId, cancellationToken)
                ?? throw new EntityNotFoundException(command.EventId);

            var participants = await _partRepo
                .ListAsync(
                   new ParticipantByUserIdSpecification(command.UserId),
                   cancellationToken);

            var participant = participants.SingleOrDefault()
                ?? throw new EntityNotFoundException();

            if (evt.Participants.Any(ep => ep.ParticipantId == participant.Id))
                throw new ForbiddenException("You are already registered for this event.");

            evt.AddParticipant(participant.Id);
            await _eventRepo.UpdateAsync(evt, cancellationToken);

            return Unit.Value;
        }

    }

}
