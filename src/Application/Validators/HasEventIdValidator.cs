using Events.Application.Common;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Application.Validators
{
    public class HasEventIdValidator<T> : AbstractValidator<T>
        where T : IHasEventId
    {
        public HasEventIdValidator()
        {
            RuleFor(x => x.EventId)
                .NotEmpty().WithMessage("EventId is required.");
        }
    }
}
