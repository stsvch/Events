using Events.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Application.Commands
{
    public class RegisterParticipantCommand : IRequest<Unit>, IHasEventId
    {
        public Guid EventId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTimeOffset DateOfBirth { get; set; }
    }
}
