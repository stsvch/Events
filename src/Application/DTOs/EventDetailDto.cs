using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Application.DTOs
{
    public class EventDetailDto : EventDto
    {
        public string Description { get; set; }
        public int Capacity { get; set; }
        public IEnumerable<EventImageDto> Images { get; set; }
        public int RegisteredCount { get; set; }
    }

}
