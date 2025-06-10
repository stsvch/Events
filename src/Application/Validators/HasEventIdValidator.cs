using Events.Application.Common;
using FluentValidation;

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
