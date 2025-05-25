using Events.Application.Queries;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Application.Validators
{
    public class GetEventsByParticipantQueryValidator : AbstractValidator<GetEventsByParticipantQuery>
    {
        public GetEventsByParticipantQueryValidator()
        {
            RuleFor(x => x.ParticipantId)
                .NotEmpty().WithMessage("Participant Id is required.");
        }
    }
}
