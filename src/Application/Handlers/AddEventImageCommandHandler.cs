using Events.Application.Commands;
using Events.Domain.Exceptions;
using Events.Domain.Repositories;
using MediatR;

namespace Events.Application.Handlers
{
    public class AddEventImageCommandHandler : IRequestHandler<AddEventImageCommand, Unit>
    {
        private readonly IEventRepository _repo;
        public AddEventImageCommandHandler(IEventRepository repo) => _repo = repo;

        public async Task<Unit> Handle(AddEventImageCommand command, CancellationToken cancellationToken)
        {
            var existing = await _repo.GetByIdAsync(command.EventId, cancellationToken);
            if (existing == null)
                throw new EntityNotFoundException(command.EventId);

            existing.AddImage(command.Url);
            await _repo.UpdateAsync(existing, cancellationToken);

            return Unit.Value;
        }
    }
}
