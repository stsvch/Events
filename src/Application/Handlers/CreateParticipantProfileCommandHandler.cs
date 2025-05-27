using Events.Application.Commands;
using Events.Domain.Entities;
using Events.Domain.Repositories;
using Events.Domain.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Application.Handlers
{
    public class CreateParticipantProfileCommandHandler
        : IRequestHandler<CreateParticipantProfileCommand, Unit>
    {
        private readonly IParticipantRepository _participantRepo;

        public CreateParticipantProfileCommandHandler(IParticipantRepository participantRepo)
            => _participantRepo = participantRepo;

        public async Task<Unit> Handle(CreateParticipantProfileCommand cmd, CancellationToken ct)
        {
            var participant = new Participant(
                new PersonName(cmd.FirstName, cmd.LastName),
                new EmailAddress(cmd.Email),
                cmd.DateOfBirth,
                cmd.UserId);

            await _participantRepo.AddAsync(participant);
            return Unit.Value;
        }
    }
}
