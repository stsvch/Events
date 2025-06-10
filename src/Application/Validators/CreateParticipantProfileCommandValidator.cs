using Events.Application.Commands;
using FluentValidation;

namespace Events.Application.Validators
{
    public class CreateParticipantProfileCommandValidator
        : AbstractValidator<CreateParticipantProfileCommand>
    {
        public CreateParticipantProfileCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(50).WithMessage("First name must be at most 50 characters long.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(50).WithMessage("Last name must be at most 50 characters long.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email must be a valid email address.")
                .MaximumLength(100).WithMessage("Email must be at most 100 characters long.");

            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("Date of birth is required.")
                .LessThan(DateTimeOffset.UtcNow)
                    .WithMessage("Date of birth must be in the past.");
        }
    }
}
