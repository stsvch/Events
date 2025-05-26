using Events.Application.Common;
using MediatR;


namespace Events.Application.Commands
{
    public class UnregisterParticipantCommand : IRequest<Unit>, IHasEventId, IHasParticipantId
    {
        public Guid EventId { get; set; }
        public Guid ParticipantId { get; set; }

        public string UserId { get; set; } = null!;
    }
}
