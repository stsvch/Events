using AutoMapper;
using Events.Application.DTOs;
using Events.Application.Queries;
using Events.Domain.Entities;
using Events.Domain.Repositories;
using Events.Domain.Specifications;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
            ISpecification<Event> spec = Specification<Event>.True;

            if (query.StartDate.HasValue)
                spec = spec.And(new EventDateAfterSpecification(query.StartDate.Value));

            if (query.EndDate.HasValue)
                spec = spec.And(new EventDateBeforeSpecification(query.EndDate.Value));

            if (!string.IsNullOrWhiteSpace(query.Venue))
                spec = spec.And(new EventVenueSpecification(query.Venue));

            if (query.CategoryId.HasValue)
                spec = spec.And(new EventCategorySpecification(query.CategoryId.Value));

            var filtered = (await _repo.ListAsync(spec, cancellationToken)).ToList();

            var items = filtered
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize);

            return new PagedResultDto<EventDto>
            {
                Items = _mapper.Map<IEnumerable<EventDto>>(items),
                TotalCount = filtered.Count,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            };
        }
    }
}
