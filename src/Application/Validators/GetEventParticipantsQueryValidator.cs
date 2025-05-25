using Events.Application.Queries;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Application.Validators
{
    public class GetEventParticipantsQueryValidator : AbstractValidator<GetEventParticipantsQuery>
    {
        public GetEventParticipantsQueryValidator()
        {
            RuleFor(x => x.EventId)
                .NotEmpty().WithMessage("Event Id is required.");
        }
    }
}
