using Events.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Application.Commands
{
    public class DeleteEventImageCommand : IRequest<Unit>, IHasEventId
    {
        public Guid EventId { get; set; }
        public Guid ImageId { get; set; }
    }
}
