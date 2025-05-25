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
    public interface IEventService
    {
        Task<PagedResultDto<EventDto>> GetAllEventsAsync(GetAllEventsQuery query);
        Task<PagedResultDto<EventDto>> SearchEventsAsync(SearchEventsQuery query);
        Task<EventDetailDto> GetEventByIdAsync(GetEventByIdQuery query);
        Task<EventDetailDto> GetEventByTitleAsync(GetEventByTitleQuery query);
        Task<Guid> CreateEventAsync(CreateEventCommand command);
        Task UpdateEventAsync(UpdateEventCommand command);
        Task DeleteEventAsync(DeleteEventCommand command);
        Task AddImageAsync(AddEventImageCommand command);
    }
}
