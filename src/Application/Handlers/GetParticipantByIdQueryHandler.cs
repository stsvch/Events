using AutoMapper;
using Events.Application.DTOs;
using Events.Application.Queries;
using Events.Domain.Repositories;
using Events.Domain.Specifications;
using MediatR;

namespace Events.Application.Handlers
{
    public class GetParticipantByIdQueryHandler
        : IRequestHandler<GetParticipantByIdQuery, ParticipantDto?>
    {
        private readonly IParticipantRepository _repo;
        private readonly IMapper _mapper;

        public GetParticipantByIdQueryHandler(IParticipantRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<ParticipantDto?> Handle(
            GetParticipantByIdQuery request,
            CancellationToken cancellationToken)
        {
            var spec = new ParticipantByIdSpecification(request.ParticipantId);
            var participant = await _repo.GetBySpecAsync(spec, cancellationToken);

            if (participant is null)
                return null;

            return _mapper.Map<ParticipantDto>(participant);
        }
    }
}
