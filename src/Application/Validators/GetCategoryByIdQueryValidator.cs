using Events.Application.Queries;
using Events.Domain.Repositories;
using Events.Domain.Specifications;
using FluentValidation;

namespace Events.Application.Validators
{
    public class GetCategoryByIdQueryValidator
        : AbstractValidator<GetCategoryByIdQuery>
    {
        public GetCategoryByIdQueryValidator(ICategoryRepository repo)
        {
            Include(new HasIdValidator<GetCategoryByIdQuery>());
            RuleFor(x => x.Id)
                .MustAsync(async (id, ct) =>
                    await repo
                        .GetByIdAsync(id, ct) != null)

                .WithMessage("Category with Id '{PropertyValue}' was not found.");
        }
    }
}
