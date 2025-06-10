using Events.Application.Common;
using FluentValidation;

namespace Events.Application.Validators
{
    public class HasIdValidator<T> : AbstractValidator<T>
        where T : IHasId
    {
        public HasIdValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id is required.");
        }
    }
}
