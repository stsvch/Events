using Events.Application.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Application.Validators
{
    public class UnregisterParticipantCommandValidator : AbstractValidator<UnregisterParticipantCommand>
    {
        public UnregisterParticipantCommandValidator()
        {
            RuleFor(x => x.EventId)
                .NotEmpty().WithMessage("EventId is required.");

            RuleFor(x => x.ParticipantId)
                .NotEmpty().WithMessage("ParticipantId is required.");
        }
    }
}
