using Events.Application.Common;
using MediatR;

namespace Events.Application.Commands
{
    public class DeleteEventImageCommand : IRequest<Unit>, IHasEventId
    {
        public Guid EventId { get; set; }
        public Guid ImageId { get; set; }
    }
}
