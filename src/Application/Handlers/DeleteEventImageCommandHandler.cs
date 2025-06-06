﻿using Events.Application.Commands;
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
    public class DeleteEventImageCommandHandler: IRequestHandler<DeleteEventImageCommand, Unit>
    {
        private readonly IEventRepository _eventRepo;
        private readonly IEventImageRepository _imageRepo;

        public DeleteEventImageCommandHandler(IEventRepository eventRepo, IEventImageRepository imageRepo)
        {
            _eventRepo = eventRepo;
            _imageRepo = imageRepo;
        }

        public async Task<Unit> Handle(DeleteEventImageCommand command, CancellationToken cancellationToken)
        {
            var evt = await _eventRepo.GetByIdAsync(command.EventId,cancellationToken)?? throw new EntityNotFoundException(command.EventId);

            var img = await _imageRepo.GetByIdAsync(command.ImageId, cancellationToken) ?? throw new EntityNotFoundException(command.ImageId);

            if (img.EventId != command.EventId)
                throw new InvariantViolationException($"Image {command.ImageId} does not belong to Event {command.EventId}.");

            await _imageRepo.DeleteAsync(command.ImageId, cancellationToken);

            return Unit.Value;
        }
    }
}
