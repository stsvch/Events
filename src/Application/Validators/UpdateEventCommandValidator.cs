using Events.Application.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Application.Validators
{
    public class UpdateEventCommandValidator : AbstractValidator<UpdateEventCommand>
    {
        public UpdateEventCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Event Id is required.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200);

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(2000);

            RuleFor(x => x.Date)
                .GreaterThan(DateTimeOffset.UtcNow)
                .WithMessage("Event date must be in the future.");

            RuleFor(x => x.Venue)
                .NotEmpty().WithMessage("Venue is required.")
                .MaximumLength(300);

            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("CategoryId is required.");

            RuleFor(x => x.Capacity)
                .GreaterThan(0).WithMessage("Capacity must be at least 1.");
        }
    }
}