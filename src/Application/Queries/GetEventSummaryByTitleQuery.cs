using Events.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Application.Queries
{
    public class GetEventSummaryByTitleQuery : IRequest<EventDto>
    {
        public string Title { get; }
        public GetEventSummaryByTitleQuery(string title) => Title = title;
    }
}
