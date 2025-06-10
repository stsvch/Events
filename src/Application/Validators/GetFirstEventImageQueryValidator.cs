using Events.Application.Queries;
using Events.Domain.Repositories;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Application.Validators
{
    public class GetFirstEventImageQueryValidator
        : AbstractValidator<GetFirstEventImageQuery>
    {
        public GetFirstEventImageQueryValidator(IEventRepository eventRepo)
        {

            RuleFor(x => x.EventId)
                .MustAsync(async (id, ct) =>
                    await eventRepo.GetByIdAsync(id, ct)!=null
                )
                .WithMessage("Event with Id '{PropertyValue}' was not found.");
        }
    }
}
