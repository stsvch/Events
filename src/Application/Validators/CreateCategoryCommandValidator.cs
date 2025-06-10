using Events.Application.Commands;
using FluentValidation;

namespace Events.Application.Validators
{
    public class CreateCategoryCommandValidator
        : AbstractValidator<CreateCategoryCommand>
    {
        public CreateCategoryCommandValidator()
        {
            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("The category name must not be empty.")
                .MaximumLength(100);
        }
    }
}
