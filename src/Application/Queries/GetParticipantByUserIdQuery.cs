using Events.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Application.Queries
{
    public class GetParticipantByUserIdQuery : IRequest<ParticipantDto?>
    {
        public string UserId { get; init; } = default!;
    }
}
