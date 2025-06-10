using Events.Application.DTOs;
using MediatR;

namespace Events.Application.Queries
{
    public class GetParticipantByIdQuery : IRequest<ParticipantDto?>
    {
        public Guid ParticipantId { get; init; }
    }
}
