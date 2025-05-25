using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Application.DTOs
{
    public class EventImageDto
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        public string Caption { get; set; }
        public DateTimeOffset UploadedAt { get; set; }
    }
}
