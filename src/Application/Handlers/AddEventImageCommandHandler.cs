using AutoMapper;
using Events.Application.Commands;
using Events.Domain.Entities;
using Events.Domain.Exceptions;
using Events.Domain.Repositories;
using MediatR;

namespace Events.Application.Handlers
{
    public class AddEventImageCommandHandler
            : IRequestHandler<AddEventImageCommand, Unit>
    {
        private readonly IEventRepository _eventRepo;
        private readonly IEventImageRepository _imageRepo;
        private readonly IMapper _mapper;

        public AddEventImageCommandHandler(
            IEventRepository eventRepo,
            IEventImageRepository imageRepo,
            IMapper mapper)
        {
            _eventRepo = eventRepo;
            _imageRepo = imageRepo;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(
            AddEventImageCommand command,
            CancellationToken cancellationToken)
        {
            var evt = await _eventRepo.GetByIdAsync(
                          command.EventId,
                          cancellationToken)
                      ?? throw new EntityNotFoundException(command.EventId);

            var image = _mapper.Map<EventImage>(command);

            await _imageRepo.AddAsync(image, cancellationToken);

            return Unit.Value;
        }
    }
}
