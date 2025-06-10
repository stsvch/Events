using Events.Application.Commands;
using FluentValidation;

namespace Events.Application.Validators
{
    public class DeleteCategoryCommandValidator
        : AbstractValidator<DeleteCategoryCommand>
    {
        public DeleteCategoryCommandValidator()
        {
            Include(new HasIdValidator<DeleteCategoryCommand>());
        }
    }
}
