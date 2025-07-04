﻿using Events.Application.DTOs;
using Events.Domain.Common;
using MediatR;

namespace Events.Application.Queries
{
    public class SearchEventsQuery : IRequest<PagedResultDto<EventDto>>
    {
        public string? Title { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public string Venue { get; set; }
        public Guid? CategoryId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public SpecificationCombineMode CombineMode { get; set; } = SpecificationCombineMode.And;
    }
}
