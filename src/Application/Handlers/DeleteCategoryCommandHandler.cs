using Events.Application.Commands;
using Events.Domain.Repositories;
using MediatR;

namespace Events.Application.Handlers
{
    public class DeleteCategoryCommandHandler
        : IRequestHandler<DeleteCategoryCommand, Unit>
    {
        private readonly ICategoryRepository _repo;
        public DeleteCategoryCommandHandler(ICategoryRepository repo)
            => _repo = repo;

        public async Task<Unit> Handle(
            DeleteCategoryCommand command,
            CancellationToken cancellationToken)
        {
            await _repo.DeleteAsync(command.Id, cancellationToken);
            return Unit.Value;
        }
    }
}
