using AutoMapper;
using Events.Application.DTOs;
using Events.Application.Queries;
using Events.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Application.Handlers
{
    public class GetEventByTitleQueryHandler : IRequestHandler<GetEventByTitleQuery, EventDetailDto>
    {
        private readonly IEventRepository _repo;
        private readonly IMapper _mapper;
        public GetEventByTitleQueryHandler(IEventRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<EventDetailDto> Handle(GetEventByTitleQuery query, CancellationToken cancellationToken)
        {
            var evt = await _repo.GetByTitleAsync(query.Title, cancellationToken);
            if (evt == null)
                throw new KeyNotFoundException($"Event with title '{query.Title}' not found.");

            return _mapper.Map<EventDetailDto>(evt);
        }
    }
}
