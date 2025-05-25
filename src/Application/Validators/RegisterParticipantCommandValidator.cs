using Events.Application.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Application.Validators
{
    public class RegisterParticipantCommandValidator : AbstractValidator<RegisterParticipantCommand>
    {
        public RegisterParticipantCommandValidator()
        {
            Include(new HasEventIdValidator<RegisterParticipantCommand>());

            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("FullName is required.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email must be valid.");

            RuleFor(x => x.DateOfBirth)
                .LessThan(DateTimeOffset.Now).WithMessage("DateOfBirth must be in the past.");
            RuleFor(x => x.FullName)
                .Must(name => name.Split(' ', 2).Length == 2 && !string.IsNullOrWhiteSpace(name.Split(' ', 2)[1]))
                .WithMessage("Enter your first and last name separated by a space.");
        }
    }
}
