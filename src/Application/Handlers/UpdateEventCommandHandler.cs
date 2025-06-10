using Events.Application.Commands;
using Events.Domain.Exceptions;
using Events.Domain.Repositories;
using MediatR;

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

            existing.UpdateDetails(
                command.Title,
                command.Description,
                command.Date,
                command.Venue,
                command.CategoryId,
                command.Capacity);

            await _repo.UpdateAsync(existing, cancellationToken);

            return Unit.Value;
        }
    }
}
