using Events.Application.Commands;
using FluentValidation;
using System;

namespace Events.Application.Validators
{
    public class RegisterParticipantCommandValidator : AbstractValidator<RegisterParticipantCommand>
    {
        public RegisterParticipantCommandValidator()
        {
            Include(new HasEventIdValidator<RegisterParticipantCommand>());

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required (must be authenticated).");

            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("FullName is required.")
                .Must(name => name.Split(' ', 2).Length == 2)
                .WithMessage("Enter your first and last name separated by a space.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email must be valid.");

            RuleFor(x => x.DateOfBirth)
                .LessThan(DateTimeOffset.Now).WithMessage("DateOfBirth must be in the past.");
        }
    }
}
