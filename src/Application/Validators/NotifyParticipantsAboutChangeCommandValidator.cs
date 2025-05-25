using Events.Application.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Application.Validators
{
    public class NotifyParticipantsAboutChangeCommandValidator : AbstractValidator<NotifyParticipantsAboutChangeCommand>
    {
        public NotifyParticipantsAboutChangeCommandValidator()
        {
            RuleFor(x => x.EventId)
                .NotEmpty().WithMessage("EventId is required.");

            RuleFor(x => x.Message)
                .NotEmpty().WithMessage("Message is required.")
                .MaximumLength(2000);
        }
    }
}
