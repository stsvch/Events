using Events.Application.Common;
using FluentValidation;

namespace Events.Application.Validators
{
    public class HasParticipantIdValidator<T> : AbstractValidator<T>
        where T : IHasParticipantId
    {
        public HasParticipantIdValidator()
        {
            RuleFor(x => x.ParticipantId)
                .NotEmpty().WithMessage("ParticipantId is required.");
        }
    }
}
