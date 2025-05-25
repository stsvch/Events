using Events.Application.Commands;
using Events.Application.DTOs;
using Events.Application.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Application.Interfaces
{
    public interface IParticipantService
    {
        Task RegisterAsync(RegisterParticipantCommand command);
        Task UnregisterAsync(UnregisterParticipantCommand command);
        Task<IEnumerable<ParticipantDto>> GetEventParticipantsAsync(GetEventParticipantsQuery query);
    }
}
