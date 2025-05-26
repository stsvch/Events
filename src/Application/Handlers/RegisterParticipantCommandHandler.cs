using Events.Application.Commands;
using Events.Domain.Entities;
using Events.Domain.Exceptions;
using Events.Domain.Repositories;
using Events.Domain.Specifications;
using Events.Domain.ValueObjects;
using MediatR;

namespace Events.Application.Handlers
{
    public class RegisterParticipantCommandHandler
        : IRequestHandler<RegisterParticipantCommand, Unit>
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
                .GetByIdAsync(command.EventId, cancellationToken)
                ?? throw new EntityNotFoundException(command.EventId);

            foreach (var ep in evt.Participants)
            {
                var p = await _partRepo
                    .GetByIdAsync(ep.ParticipantId, cancellationToken);
                if (p != null && p.UserId == command.UserId)
                    throw new ForbiddenException("You are already registered for this event.");
            }

            var parts = command.FullName.Split(' ', 2);
            var nameVo = new PersonName(parts[0], parts.ElementAtOrDefault(1) ?? "");
            var emailVo = new EmailAddress(command.Email);

            Participant participant;
            var existing = (await _partRepo
                .ListAsync(new ParticipantByEmailSpecification(emailVo.Value), cancellationToken))
                .FirstOrDefault();

            if (existing != null)
            {
                participant = existing;
                if (participant.UserId != command.UserId)
                {
                    participant.UserId = command.UserId;
                    await _partRepo.UpdateAsync(participant, cancellationToken);
                }
            }
            else
            {
                participant = new Participant(
                    nameVo,
                    emailVo,
                    command.DateOfBirth,
                    command.UserId);
                await _partRepo.AddAsync(participant, cancellationToken);
            }

            evt.AddParticipant(participant.Id);
            await _eventRepo.UpdateAsync(evt, cancellationToken);

            return Unit.Value;
        }
    }
}
