using AutoMapper;
using Events.Application.DTOs;
using Events.Application.Queries;
using Events.Domain.Common;
using Events.Domain.Entities;
using Events.Domain.Repositories;
using Events.Domain.Specifications;
using MediatR;

namespace Events.Application.Handlers
{
    public class SearchEventsQueryHandler : IRequestHandler<SearchEventsQuery, PagedResultDto<EventDto>>
    {
        private readonly IEventRepository _repo;
        private readonly IMapper _mapper;

        public SearchEventsQueryHandler(IEventRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
        public async Task<PagedResultDto<EventDto>> Handle(SearchEventsQuery query, CancellationToken cancellationToken)
        {
            List<ISpecification<Event>> specs = new();

            if (query.StartDate.HasValue)
                specs.Add(new EventDateAfterSpecification(query.StartDate.Value));
            if (query.EndDate.HasValue)
                specs.Add(new EventDateBeforeSpecification(query.EndDate.Value));
            if (!string.IsNullOrWhiteSpace(query.Venue))
                specs.Add(new EventVenueSpecification(query.Venue!));
            if (query.CategoryId.HasValue)
                specs.Add(new EventCategorySpecification(query.CategoryId.Value));
            if (!string.IsNullOrWhiteSpace(query.Title))
                specs.Add(new EventTitleSpecification(query.Title));

            ISpecification<Event> combinedSpec;

            if (specs.Count == 0)
            {
                combinedSpec = Specification<Event>.True;
            }
            else if (query.CombineMode == SpecificationCombineMode.And)
            {
                combinedSpec = Specification<Event>.True;
                foreach (var s in specs)
                    combinedSpec = combinedSpec.And(s);
            }
            else 
            {
                combinedSpec = Specification<Event>.False;
                foreach (var s in specs)
                    combinedSpec = combinedSpec.Or(s);
            }

            var paged = await _repo.ListAsync(
                combinedSpec,
                query.PageNumber,
                query.PageSize,
                includeDetails: true,
                cancellationToken
            );

            return new PagedResultDto<EventDto>
            {
                Items = _mapper.Map<IEnumerable<EventDto>>(paged.Items),
                TotalCount = paged.TotalCount,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            };
        }
    }

}
