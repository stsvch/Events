using Events.Application.Commands;
using Events.Domain.Entities;
using Events.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Application.Handlers
{
    public class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, Guid>
    {
        private readonly IEventRepository _repo;
        public CreateEventCommandHandler(IEventRepository repo) => _repo = repo;

        public async Task<Guid> Handle(CreateEventCommand command, CancellationToken cancellationToken)
        {
            var evt = new Event(
                Guid.NewGuid(),
                command.Title,
                command.Description,   
                command.Date,
                command.Venue,
                command.CategoryId,
                command.Capacity);

            await _repo.AddAsync(evt, cancellationToken);
            return evt.Id;
        }
    }
}
