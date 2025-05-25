using AutoMapper;
using Events.Application.DTOs;
using Events.Application.Queries;
using Events.Domain.Exceptions;
using Events.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Application.Handlers
{
    public class GetEventSummaryByTitleQueryHandler : IRequestHandler<GetEventSummaryByTitleQuery, EventDto>
    {
        private readonly IEventRepository _repo;
        private readonly IMapper _mapper;

        public GetEventSummaryByTitleQueryHandler(IEventRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<EventDto> Handle(GetEventSummaryByTitleQuery request, CancellationToken cancellationToken)
        {
            var entity = await _repo.GetByTitleAsync(request.Title);
            if (entity == null)
                throw new EntityNotFoundException(Guid.NewGuid());
            return _mapper.Map<EventDto>(entity);
        }
    }
}
