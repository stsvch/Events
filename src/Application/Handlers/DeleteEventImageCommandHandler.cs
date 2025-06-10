using Events.Application.Commands;
using Events.Domain.Exceptions;
using Events.Domain.Repositories;
using MediatR;

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
            var evt = await _eventRepo.GetByIdAsync(command.EventId,cancellationToken);

            var img = await _imageRepo.GetByIdAsync(command.ImageId, cancellationToken);

            await _imageRepo.DeleteAsync(command.ImageId, cancellationToken);

            return Unit.Value;
        }
    }
}
