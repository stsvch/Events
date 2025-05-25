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
    public class GetEventSummaryQueryHandler : IRequestHandler<GetEventSummaryQuery, EventDto>
    {
        private readonly IEventRepository _repo;
        private readonly IMapper _mapper;

        public GetEventSummaryQueryHandler(IEventRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<EventDto> Handle(GetEventSummaryQuery request, CancellationToken cancellationToken)
        {
            var entity = await _repo.GetByIdAsync(request.Id);
            if (entity == null)
                throw new EntityNotFoundException( request.Id);
            return _mapper.Map<EventDto>(entity);
        }
    }
}
