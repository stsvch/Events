using Events.Application.Commands;
using FluentValidation;

namespace Events.Application.Validators
{
    public class NotifyParticipantsAboutChangeCommandValidator : AbstractValidator<NotifyParticipantsAboutChangeCommand>
    {
        public NotifyParticipantsAboutChangeCommandValidator()
        {
            Include(new HasEventIdValidator<NotifyParticipantsAboutChangeCommand>());

            RuleFor(x => x.Message)
                .NotEmpty().WithMessage("Message is required.");
        }
    }
}
