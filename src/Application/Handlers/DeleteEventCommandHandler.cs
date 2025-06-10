using Events.Application.Commands;
using Events.Domain.Repositories;
using MediatR;

namespace Events.Application.Handlers
{
    public class DeleteEventCommandHandler : IRequestHandler<DeleteEventCommand, Unit>
    {
        private readonly IEventRepository _repo;
        public DeleteEventCommandHandler(IEventRepository repo) => _repo = repo;

        public async Task<Unit> Handle(DeleteEventCommand command, CancellationToken cancellationToken)
        {
            await _repo.DeleteAsync(command.Id, cancellationToken);
            return Unit.Value;
        }
    }
}
