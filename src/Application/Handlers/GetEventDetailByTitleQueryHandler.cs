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
    public class GetEventDetailByTitleQueryHandler : IRequestHandler<GetEventDetailByTitleQuery, EventDetailDto>
    {
        private readonly IEventRepository _repo;
        private readonly IMapper _mapper;

        public GetEventDetailByTitleQueryHandler(IEventRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<EventDetailDto> Handle(GetEventDetailByTitleQuery request, CancellationToken cancellationToken)
        {
            var entity = await _repo.GetByTitleWithDetailsAsync(request.Title);
            if (entity == null)
                throw new EntityNotFoundException();
            return _mapper.Map<EventDetailDto>(entity);
        }
    }
}
