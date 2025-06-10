using Events.Application.Commands;
using Events.Domain.Repositories;
using Events.Domain.Specifications;
using FluentValidation;

namespace Events.Application.Validators
{
    public class UnregisterParticipantCommandValidator
        : AbstractValidator<UnregisterParticipantCommand>
    {
        public UnregisterParticipantCommandValidator(
            IEventRepository eventRepo,
            IParticipantRepository participantRepo)
        {
            Include(new HasEventIdValidator<UnregisterParticipantCommand>());

            RuleFor(x => x)
                .MustAsync(async (cmd, ct) =>
                {
                    var evt = await eventRepo
                        .GetByIdWithDetailsAsync(cmd.EventId, ct);
                    if (evt == null) return false;
                    var participant = await participantRepo
                        .GetBySpecAsync(
                            new ParticipantByUserIdSpecification(cmd.UserId),
                            ct);
                    if (participant == null) return false;
                    return evt.Participants
                        .Any(ep => ep.ParticipantId == participant.Id);
                })
                .WithMessage("You are not registered for this event.");
        }
    }
}
