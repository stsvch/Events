using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Application.DTOs
{
    public class EventDetailDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTimeOffset Date { get; set; }
        public string Venue { get; set; } = null!;
        public Guid CategoryId { get; set; }
        public int Capacity { get; set; }
        public int ParticipantCount { get; set; }
        public List<EventImageDto> Images { get; set; } = new();
    }
}
