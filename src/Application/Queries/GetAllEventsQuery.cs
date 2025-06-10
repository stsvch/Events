using Events.Application.DTOs;
using MediatR;

namespace Events.Application.Queries
{
    public class GetAllEventsQuery : IRequest<PagedResultDto<EventDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public bool IncludeDetails { get; set; } = false;
    }
}
