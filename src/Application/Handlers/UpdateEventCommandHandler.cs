using Events.Application.Commands;
using Events.Domain.Exceptions;
using Events.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Application.Handlers
{
    public class UpdateEventCommandHandler : IRequestHandler<UpdateEventCommand, Unit>
    {
        private readonly IEventRepository _repo;

        public UpdateEventCommandHandler(IEventRepository repo)
            => _repo = repo;

        public async Task<Unit> Handle(UpdateEventCommand command, CancellationToken cancellationToken)
        {
            var existing = await _repo.GetByIdAsync(command.Id, cancellationToken);
            if (existing == null)
                throw new EntityNotFoundException(command.Id);

            existing.UpdateDetails(
                command.Title,
                command.Description,
                command.Date,
                command.Venue,
                command.CategoryId,
                command.Capacity);

            // нужно посмотреть сохр изм 

            return Unit.Value;
        }
    }
}
