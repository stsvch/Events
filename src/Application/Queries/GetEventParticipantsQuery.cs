using Events.Application.Common;
using Events.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Application.Queries
{
    public class GetEventParticipantsQuery : IRequest<IEnumerable<ParticipantDto>>, IHasEventId
    {
        public Guid EventId { get; set; }
    }
}
