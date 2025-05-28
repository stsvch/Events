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
    public class GetEventsByParticipantQueryHandler : IRequestHandler<GetEventsByParticipantQuery, IEnumerable<EventDto>>
    {
        private readonly IParticipantRepository _partRepo;
        private readonly IEventRepository _eventRepo;
        private readonly IMapper _mapper;

        public GetEventsByParticipantQueryHandler(IEventRepository eventRepo,IMapper mapper, IParticipantRepository partRepo)
        {
            _eventRepo = eventRepo;
            _mapper = mapper;
            _partRepo = partRepo;
        }

        public async Task<IEnumerable<EventDto>> Handle( GetEventsByParticipantQuery query, CancellationToken cancellationToken)
        {
            var participant = await _partRepo.GetBySpecAsync(new ParticipantByUserIdSpecification(query.UserId), cancellationToken);
            var spec = new EventByParticipantSpecification(participant.Id);
            var events = await _eventRepo.ListAsync(spec, cancellationToken);
            return _mapper.Map<IEnumerable<EventDto>>(events);
        }
    }
}
