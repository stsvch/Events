using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Application.Commands
{
    public class UnregisterParticipantCommand : IRequest<Unit>
    {
        public Guid EventId { get; set; }
        public Guid ParticipantId { get; set; }
    }
}
