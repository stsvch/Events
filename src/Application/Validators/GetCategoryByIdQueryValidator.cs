using Events.Application.Queries;
using FluentValidation;

namespace Events.Application.Validators
{
    public class GetCategoryByIdQueryValidator
        : AbstractValidator<GetCategoryByIdQuery>
    {
        public GetCategoryByIdQueryValidator()
        {
            Include(new HasIdValidator<GetCategoryByIdQuery>());
        }
    }
}
