using Events.Application.Common;
using Events.Application.DTOs;
using MediatR;

namespace Events.Application.Queries
{
    public class GetEventsByParticipantQuery : IRequest<IEnumerable<EventDto>>
    {
        public string UserId { get; set; }
        public GetEventsByParticipantQuery(string userId)
        {
            UserId = userId;
        }
    }
}
