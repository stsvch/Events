﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Application.DTOs
{
    public class ParticipantDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTimeOffset DateOfBirth { get; set; }
    }
}
