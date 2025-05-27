using AutoMapper;
using Events.Application.DTOs;
using Events.Application.Queries;
using Events.Domain.Exceptions;
using Events.Domain.Repositories;
using MediatR;

namespace Events.Application.Handlers
{
    public class GetCategoryByIdQueryHandler
            : IRequestHandler<GetCategoryByIdQuery, CategoryDto>
    {
        private readonly ICategoryRepository _repo;
        private readonly IMapper _mapper;

        public GetCategoryByIdQueryHandler(
            ICategoryRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<CategoryDto> Handle(
            GetCategoryByIdQuery request,
            CancellationToken cancellationToken)
        {
            var entity = await _repo.GetByIdAsync(request.Id, cancellationToken);
            if (entity == null)
                throw new EntityNotFoundException(request.Id);
            return _mapper.Map<CategoryDto>(entity);
        }
    }
}
