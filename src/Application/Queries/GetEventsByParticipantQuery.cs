﻿using Events.Application.Common;
using Events.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Application.Queries
{
    public class GetEventsByParticipantQuery : IRequest<IEnumerable<EventDto>>
    {
        public string UserId { get; set; }
        public GetEventsByParticipantQuery(string userId)
        {
            UserId = userId;
        }
    }
}
