using Events.Application.DTOs;
using MediatR;

namespace Events.Application.Queries
{
    public class GetParticipantByUserIdQuery : IRequest<ParticipantDto?>
    {
        public string UserId { get; init; } = default!;
    }
}
