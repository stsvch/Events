using Events.Application.Commands;
using Events.Domain.Entities;
using Events.Domain.Repositories;
using MediatR;

namespace Events.Application.Handlers
{
    namespace Events.Application.Handlers
    {
        public class CreateCategoryCommandHandler
            : IRequestHandler<CreateCategoryCommand, Guid>
        {
            private readonly ICategoryRepository _repo;
            public CreateCategoryCommandHandler(ICategoryRepository repo)
                => _repo = repo;

            public async Task<Guid> Handle(
                CreateCategoryCommand command,
                CancellationToken cancellationToken)
            {
                var category = new Category(command.Name);
                await _repo.AddAsync(category, cancellationToken);
                return category.Id;
            }
        }
    }
}
