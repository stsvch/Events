using Events.Application.Common;
using Events.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Application.Queries
{
    public class GetEventDetailQuery : IRequest<EventDetailDto>, IHasId
    {
        public Guid Id { get; }
        public GetEventDetailQuery(Guid id) => Id = id;
    }
}
