using Events.Application.Queries;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Application.Validators
{
    public class GetEventSummaryByTitleQueryValidator : AbstractValidator<GetEventSummaryByTitleQuery>
    {
        public GetEventSummaryByTitleQueryValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200).WithMessage("Title must be at most 200 characters.");
        }
    }
}
