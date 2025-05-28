using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Application.Queries
{
    public class GetEventImagesQuery : IRequest<IEnumerable<string>>
    {
        public Guid EventId { get; set; }
    }
}
