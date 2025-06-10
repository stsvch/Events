using Events.Application.DTOs;
using MediatR;

namespace Events.Application.Queries
{
    public class GetAllParticipantsQuery : IRequest<IEnumerable<ParticipantDto>> { }
}
