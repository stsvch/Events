using AutoMapper;
using Events.Application.DTOs;
using Events.Application.Queries;
using Events.Domain.Entities;
using Events.Domain.Repositories;
using Events.Domain.Specifications;
using MediatR;

namespace Events.Application.Handlers
{
    public class GetAllParticipantsQueryHandler : IRequestHandler<GetAllParticipantsQuery, IEnumerable<ParticipantDto>>
    {
        private readonly IParticipantRepository _repo;
        private readonly IMapper _mapper;

        public GetAllParticipantsQueryHandler(IParticipantRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ParticipantDto>> Handle( GetAllParticipantsQuery request,CancellationToken cancellationToken)
        {
            var spec = Specification<Participant>.True;
            var parts = await _repo.ListAsync(spec, cancellationToken);
            return _mapper.Map<IEnumerable<ParticipantDto>>(parts);
        }
    }
}
