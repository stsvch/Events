using AutoMapper;
using Events.Application.Commands;
using Events.Domain.Entities;
using Events.Domain.Repositories;
using MediatR;

namespace Events.Application.Handlers
{
    public class CreateParticipantProfileCommandHandler
            : IRequestHandler<CreateParticipantProfileCommand, Unit>
    {
        private readonly IParticipantRepository _participantRepo;
        private readonly IMapper _mapper;

        public CreateParticipantProfileCommandHandler(
            IParticipantRepository participantRepo,
            IMapper mapper)
        {
            _participantRepo = participantRepo;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(
            CreateParticipantProfileCommand cmd,
            CancellationToken ct)
        {
            var participant = _mapper.Map<Participant>(cmd);

            await _participantRepo.AddAsync(participant, ct);
            return Unit.Value;
        }
    }
}
