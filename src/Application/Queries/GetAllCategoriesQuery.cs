using Events.Application.DTOs;
using MediatR;

namespace Events.Application.Queries
{
    public class GetAllCategoriesQuery
        : IRequest<IEnumerable<CategoryDto>>
    { }
}
