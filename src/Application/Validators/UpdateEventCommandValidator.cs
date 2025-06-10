using Events.Application.Commands;
using Events.Domain.Repositories;
using FluentValidation;

namespace Events.Application.Validators
{
    public class UpdateEventCommandValidator : AbstractValidator<UpdateEventCommand>
    {
        public UpdateEventCommandValidator(
            IEventRepository eventRepo,
            ICategoryRepository categoryRepo)
        {
            Include(new HasIdValidator<UpdateEventCommand>());

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200).WithMessage("Title must be at most 200 characters.");

            RuleFor(x => x.Date)
                .GreaterThan(DateTimeOffset.Now).WithMessage("Date must be in the future.");

            RuleFor(x => x.Venue)
                .NotEmpty().WithMessage("Venue is required.");

            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("CategoryId is required.");

            RuleFor(x => x.Capacity)
                .GreaterThan(0).WithMessage("Capacity must be greater than zero.");

            RuleFor(x => x.Id)
                .MustAsync(async (id, ct) =>
                    await eventRepo.GetByIdAsync(id, ct) != null
                )
                .WithMessage("Event with Id '{PropertyValue}' was not found.");
        }
    }
}