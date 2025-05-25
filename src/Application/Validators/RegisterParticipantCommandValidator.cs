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
            RuleFor(x => x.EventId)
                .NotEmpty().WithMessage("EventId is required.");

            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Participant full name is required.")
                .MaximumLength(200);

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email must be a valid email address.");

            RuleFor(x => x.DateOfBirth)
                .LessThan(DateTimeOffset.UtcNow.AddYears(-1))
                .WithMessage("Date of birth must be at least 1 year ago.");

        }
    }
}
