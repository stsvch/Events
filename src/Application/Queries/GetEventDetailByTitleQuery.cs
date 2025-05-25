using Events.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Application.Queries
{
    public class GetEventDetailByTitleQuery : IRequest<EventDetailDto>
    {
        public string Title { get; }
        public GetEventDetailByTitleQuery(string title) => Title = title;
    }
}
