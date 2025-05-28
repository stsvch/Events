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
    public class GetParticipantByUserIdQueryHandler
        : IRequestHandler<GetParticipantByUserIdQuery, ParticipantDto?>
    {
        private readonly IParticipantRepository _repo;
        private readonly IMapper _mapper;

        public GetParticipantByUserIdQueryHandler(IParticipantRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<ParticipantDto?> Handle(
            GetParticipantByUserIdQuery request,
            CancellationToken cancellationToken)
        {
            var spec = new ParticipantByUserIdSpecification(request.UserId);

            var participant = await _repo.GetBySpecAsync(spec, cancellationToken);


            if (participant is null)
                return null;

            return _mapper.Map<ParticipantDto>(participant);
        }
    }
}