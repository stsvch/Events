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
        }
    }
}
