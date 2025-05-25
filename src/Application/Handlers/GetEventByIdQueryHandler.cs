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
    public class GetEventByIdQueryHandler : IRequestHandler<GetEventByIdQuery, EventDetailDto>
    {
        private readonly IEventRepository _repo;
        private readonly IMapper _mapper;
        public GetEventByIdQueryHandler(IEventRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<EventDetailDto> Handle(GetEventByIdQuery query, CancellationToken cancellationToken)
        {
            var evt = await _repo.GetByIdAsync(query.Id, cancellationToken);
            if (evt == null)
                throw new EntityNotFoundException(query.Id);

            return _mapper.Map<EventDetailDto>(evt);
        }
    }
}
