using Events.Application.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Application.Validators
{
    public class AddEventImageCommandValidator : AbstractValidator<AddEventImageCommand>
    {
        public AddEventImageCommandValidator()
        {
            RuleFor(x => x.EventId)
                .NotEmpty().WithMessage("EventId is required.");

            RuleFor(x => x.Url)
                .NotEmpty().WithMessage("Image URL is required.")
                .Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute))
                .WithMessage("Image URL must be a valid absolute URI.");

        }
    }
}
