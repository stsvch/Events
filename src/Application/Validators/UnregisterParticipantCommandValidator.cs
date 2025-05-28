using Events.Application.Commands;
using FluentValidation;

namespace Events.Application.Validators
{
    public class UnregisterParticipantCommandValidator : AbstractValidator<UnregisterParticipantCommand>
    {
        public UnregisterParticipantCommandValidator()
        {
            Include(new HasEventIdValidator<UnregisterParticipantCommand>());
        }
    }
}
