using AutoMapper;
using Events.Application.DTOs;
using Events.Application.Queries;
using Events.Domain.Repositories;
using Events.Domain.Specifications;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Application.Handlers
{
    public class GetEventParticipantsQueryHandler : IRequestHandler<GetEventParticipantsQuery, IEnumerable<ParticipantDto>>
    {
        private readonly IParticipantRepository _repo;
        private readonly IMapper _mapper;
        public GetEventParticipantsQueryHandler(IParticipantRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ParticipantDto>> Handle(GetEventParticipantsQuery query, CancellationToken cancellationToken)
        {
            var spec = new ParticipantByEventSpecification(query.EventId); 
            var parts = await _repo.ListAsync(spec, cancellationToken);
            return _mapper.Map<IEnumerable<ParticipantDto>>(parts);
        }
    }
}
