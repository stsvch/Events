using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Application.DTOs
{
    public class EventDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public DateTimeOffset Date { get; set; }
        public string Venue { get; set; }
        public Guid CategoryId { get; set; }
    }
}
