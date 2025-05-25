using Events.Application.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Application.Validators
{
    public class DeleteEventCommandValidator : HasIdValidator<DeleteEventCommand> { }
}
