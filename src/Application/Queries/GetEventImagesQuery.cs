using Events.Application.DTOs;
using MediatR;

namespace Events.Application.Queries
{
    public class GetEventImagesQuery : IRequest<IEnumerable<EventImageDto>>
    {
        public Guid EventId { get; set; }
    }
}
