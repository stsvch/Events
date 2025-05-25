using Events.Application.Commands;
using Events.Domain.Entities;
using Events.Domain.Exceptions;
using Events.Domain.Repositories;
using Events.Domain.Specifications;
using Events.Domain.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Application.Handlers
{
    public class RegisterParticipantCommandHandler
        : IRequestHandler<RegisterParticipantCommand, Unit>
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
            // 1) Загрузить событие
            var evt = await _eventRepo.GetByIdAsync(command.EventId, cancellationToken)
                      ?? throw new EntityNotFoundException(command.EventId);

            // 2) Подготовить VO
            var parts = command.FullName.Split(' ', 2);
            var nameVo = new PersonName(parts[0], parts.ElementAtOrDefault(1) ?? "");
            var emailVo = new EmailAddress(command.Email);

            // 3) Ищем участника по email
            var existing = (await _partRepo
                .ListAsync(new ParticipantByEmailSpecification(emailVo.Value), cancellationToken))
                .FirstOrDefault();

            Participant participant;
            if (existing != null)
            {
                participant = existing;
            }
            else
            {
                // 4) Если не нашли — создаём и сохраняем
                participant = new Participant(nameVo, emailVo, command.DateOfBirth);
                await _partRepo.AddAsync(participant, cancellationToken);
            }

            // 5) Добавляем ссылку в агрегат Event
            evt.AddParticipant(participant.Id);

            // 6) Сохраняем изменения в Event (EF Core под капотом вставит в таблицу связей)
            await _eventRepo.UpdateAsync(evt, cancellationToken);

            return Unit.Value;
        }

    }
}
