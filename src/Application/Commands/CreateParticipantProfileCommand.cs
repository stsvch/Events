using MediatR;

namespace Events.Application.Commands
{
    public class CreateParticipantProfileCommand : IRequest<Unit>
    {
        public string UserId { get; init; } = null!;
        public string FirstName { get; init; } = null!;
        public string LastName { get; init; } = null!;
        public string Email { get; init; } = null!;
        public DateTimeOffset DateOfBirth { get; init; }
    }
}
