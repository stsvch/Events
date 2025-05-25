using Events.Application.Common;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Application.Validators
{
    public class HasIdValidator<T> : AbstractValidator<T>
        where T : IHasId
    {
        public HasIdValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id is required.");
        }
    }
}
