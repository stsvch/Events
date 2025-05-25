using Events.Application.Queries;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Application.Validators
{
    public class SearchEventsQueryValidator : AbstractValidator<SearchEventsQuery>
    {
        public SearchEventsQueryValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0).WithMessage("PageNumber must be greater than zero.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100).WithMessage("PageSize must be between 1 and 100.");

            When(x => x.StartDate.HasValue && x.EndDate.HasValue, () =>
            {
                RuleFor(x => x.EndDate.Value)
                    .GreaterThanOrEqualTo(x => x.StartDate.Value)
                    .WithMessage("EndDate must be on or after StartDate.");
            });
        }
    }
}
