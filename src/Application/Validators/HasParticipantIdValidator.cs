using Events.Application.Common;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Application.Validators
{
    public class HasParticipantIdValidator<T> : AbstractValidator<T>
        where T : IHasParticipantId
    {
        public HasParticipantIdValidator()
        {
            RuleFor(x => x.ParticipantId)
                .NotEmpty().WithMessage("ParticipantId is required.");
        }
    }
}
