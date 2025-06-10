using Events.Application.Common;
using MediatR;

namespace Events.Application.Commands
{
    public class NotifyParticipantsAboutChangeCommand : IRequest<Unit>, IHasEventId
    {
        public Guid EventId { get; set; }
        public string EventTitle { get; set; }   
        public string Message { get; set; }
    }
}
