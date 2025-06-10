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
    public class GetEventImagesQueryValidator
        : AbstractValidator<GetEventImagesQuery>
    {
        public GetEventImagesQueryValidator(IEventRepository eventRepo)
        {
            RuleFor(x => x.EventId)
                .NotEmpty()
                .WithMessage("EventId must be provided.")

                .MustAsync(async (id, ct) =>
                    (await eventRepo.GetByIdAsync(id, ct)) != null
                )
                .WithMessage("Event with Id '{PropertyValue}' was not found.");
        }
    }
}
