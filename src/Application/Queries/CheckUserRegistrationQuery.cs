using Events.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Application.Queries
{
    public class CheckUserRegistrationQuery : IRequest<RegistrationStatusDto>
    {
        public Guid EventId { get; set; }
        public string UserId { get; set; }
    }
}
