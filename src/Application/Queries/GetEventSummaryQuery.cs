using Events.Application.Common;
using Events.Application.DTOs;
using MediatR;


namespace Events.Application.Queries
{
    public class GetEventSummaryQuery : IRequest<EventDto>, IHasId
    {
        public Guid Id { get; }
        public GetEventSummaryQuery(Guid id) => Id = id;
    }
}
