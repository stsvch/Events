using AutoMapper;
using Events.Application.DTOs;
using Events.Application.Queries;
using Events.Domain.Entities;
using Events.Domain.Repositories;
using Events.Domain.Specifications;
using MediatR;

namespace Events.Application.Handlers
{
    public class GetAllEventsQueryHandler : IRequestHandler<GetAllEventsQuery, PagedResultDto<EventDto>>
    {
        private readonly IEventRepository _repo;
        private readonly IMapper _mapper;

        public GetAllEventsQueryHandler(IEventRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<PagedResultDto<EventDto>> Handle(GetAllEventsQuery query, CancellationToken cancellationToken)
        {
            var paged = await _repo.ListAsync( Specification<Event>.True, query.PageNumber,query.PageSize,includeDetails: true,cancellationToken);

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
