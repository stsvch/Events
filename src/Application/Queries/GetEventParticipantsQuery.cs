using Events.Application.Common;
using Events.Application.DTOs;
using MediatR;

namespace Events.Application.Queries
{
    public class GetEventParticipantsQuery : IRequest<IEnumerable<ParticipantDto>>, IHasEventId
    {
        public Guid EventId { get; set; }
    }
}
