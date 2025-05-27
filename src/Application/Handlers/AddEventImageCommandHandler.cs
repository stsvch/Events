using Events.Application.Commands;
using Events.Domain.Exceptions;
using Events.Domain.Repositories;
using MediatR;

namespace Events.Application.Handlers
{
    public class AddEventImageCommandHandler
        : IRequestHandler<AddEventImageCommand, Unit>
    {
        private readonly IEventRepository _repo;

        public AddEventImageCommandHandler(IEventRepository repo)
            => _repo = repo;

        public async Task<Unit> Handle(
            AddEventImageCommand command,
            CancellationToken cancellationToken)
        {
            var evt = await _repo.GetByIdWithDetailsAsync(
                          command.EventId,
                          cancellationToken)
                      ?? throw new EntityNotFoundException(command.EventId);

            evt.AddImage(command.Url);

            await _repo.UpdateAsync(evt, cancellationToken);

            return Unit.Value;
        }
    }
}
