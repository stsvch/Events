using Events.Application.Common;
using Events.Application.DTOs;
using MediatR;

namespace Events.Application.Queries
{
    public class GetEventDetailQuery : IRequest<EventDetailDto>, IHasId
    {
        public Guid Id { get; }
        public GetEventDetailQuery(Guid id) => Id = id;
    }
}
