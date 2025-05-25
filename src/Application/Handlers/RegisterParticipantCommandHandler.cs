using Events.Application.Commands;
using Events.Domain.Entities;
using Events.Domain.Exceptions;
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
    public class RegisterParticipantCommandHandler : IRequestHandler<RegisterParticipantCommand, Unit>
    {
        private readonly IEventRepository _eventRepo;
        private readonly IParticipantRepository _partRepo;

        public RegisterParticipantCommandHandler(
            IEventRepository eventRepo,
            IParticipantRepository partRepo)
        {
            _eventRepo = eventRepo;
            _partRepo = partRepo;
        }

        public async Task<Unit> Handle(
            RegisterParticipantCommand command,
            CancellationToken cancellationToken)
        {
            var evt = await _eventRepo.GetByIdAsync(command.EventId, cancellationToken)
                      ?? throw new EntityNotFoundException(command.EventId);

            var nameParts = command.FullName.Split(' ', 2);
            var firstName = nameParts[0];
            var lastName = nameParts.Length > 1 ? nameParts[1] : string.Empty;
            var nameVo = new PersonName(firstName, lastName);

            var emailVo = new EmailAddress(command.Email);

            var participant = new Participant(
                command.EventId,
                nameVo,
                emailVo,
                command.DateOfBirth);

            evt.AddParticipant(participant);

            await _partRepo.AddAsync(participant, cancellationToken);


            return Unit.Value;
        }
    }

}
