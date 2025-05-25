using Events.Application.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Application.Interfaces
{
    public interface INotificationService
    {
        Task NotifyParticipantsAsync(NotifyParticipantsAboutChangeCommand command);
    }
}
