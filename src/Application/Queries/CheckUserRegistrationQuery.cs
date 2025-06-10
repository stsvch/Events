using Events.Application.DTOs;
using MediatR;

namespace Events.Application.Queries
{
    public class CheckUserRegistrationQuery : IRequest<RegistrationStatusDto>
    {
        public Guid EventId { get; set; }
        public string UserId { get; set; }
    }
}
