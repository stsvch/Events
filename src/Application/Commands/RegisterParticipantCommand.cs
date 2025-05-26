using Events.Application.Common;
using MediatR;

namespace Events.Application.Commands
{
    public class RegisterParticipantCommand : IRequest<Unit>, IHasEventId
    {
        public Guid EventId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTimeOffset DateOfBirth { get; set; }

        public string UserId { get; set; } = null!;
    }
}
