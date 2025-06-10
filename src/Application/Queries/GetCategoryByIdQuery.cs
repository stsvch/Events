using Events.Application.Common;
using Events.Application.DTOs;
using MediatR;

namespace Events.Application.Queries
{
    public class GetCategoryByIdQuery
        : IRequest<CategoryDto>, IHasId
    {
        public Guid Id { get; }
        public GetCategoryByIdQuery(Guid id) => Id = id;
    }
}
