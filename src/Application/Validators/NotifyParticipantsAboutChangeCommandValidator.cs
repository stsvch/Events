using Events.Application.Commands;
using Events.Domain.Repositories;
using FluentValidation;

namespace Events.Application.Validators
{
    public class NotifyParticipantsAboutChangeCommandValidator : AbstractValidator<NotifyParticipantsAboutChangeCommand>
    {
        public NotifyParticipantsAboutChangeCommandValidator(IEventRepository eventRepo)
        {
            Include(new HasEventIdValidator<NotifyParticipantsAboutChangeCommand>());

            RuleFor(x => x.Message)
                .NotEmpty().WithMessage("Message is required.");

            RuleFor(x => x.EventId)
                .MustAsync(async (id, ct) =>
                    await eventRepo.GetByIdAsync(id, ct) != null
                )
                .WithMessage("Event with Id '{PropertyValue}' was not found.");
        }
    }
}
