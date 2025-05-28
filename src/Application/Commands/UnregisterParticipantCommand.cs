using Events.Application.Common;
using MediatR;


namespace Events.Application.Commands
{
    public class UnregisterParticipantCommand : IRequest<Unit>, IHasEventId
    {
        public Guid EventId { get; set; }
        public string UserId { get; set; }
    }
}
