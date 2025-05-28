using AutoMapper;
using Events.Application.Commands;
using Events.Domain.Entities;
using Events.Domain.Repositories;
using MediatR;

namespace Events.Application.Handlers
{
    public class CreateCategoryCommandHandler
            : IRequestHandler<CreateCategoryCommand, Guid>
    {
        private readonly ICategoryRepository _repo;
        private readonly IMapper _mapper;

        public CreateCategoryCommandHandler(
            ICategoryRepository repo,
            IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<Guid> Handle(
            CreateCategoryCommand command,
            CancellationToken cancellationToken)
        {
            var category = _mapper.Map<Category>(command);

            await _repo.AddAsync(category, cancellationToken);
            return category.Id;
        }
    }
}
