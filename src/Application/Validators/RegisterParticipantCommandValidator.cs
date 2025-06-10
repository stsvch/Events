using Events.Application.Commands;
using Events.Domain.Repositories;
using Events.Domain.Specifications;
using FluentValidation;

namespace Events.Application.Validators
{
    public class RegisterParticipantCommandValidator
        : AbstractValidator<RegisterParticipantCommand>
    {
        public RegisterParticipantCommandValidator(
            IEventRepository eventRepo,
            IParticipantRepository participantRepo)
        {
            Include(new HasEventIdValidator<RegisterParticipantCommand>());

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required (must be authenticated).");
            RuleFor(x => x.UserId)
                .MustAsync(async (userId, ct) =>
                    await participantRepo
                        .GetBySpecAsync(new ParticipantByUserIdSpecification(userId), ct)!= null)
                .WithMessage("User is not found (must create profile first).");

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
                    return participant == null
                        || !evt.Participants.Any(ep => ep.ParticipantId == participant.Id);
                })
                .WithMessage("You are already registered for this event.");
        }
    }
}