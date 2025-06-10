using AutoMapper;
using Events.Application.DTOs;
using Events.Application.Queries;
using Events.Domain.Exceptions;
using Events.Domain.Repositories;
using MediatR;

namespace Events.Application.Handlers
{
    public class GetEventImagesQueryHandler: IRequestHandler<GetEventImagesQuery, IEnumerable<EventImageDto>>
    {
        private readonly IEventRepository _eventRepo;
        private readonly IEventImageRepository _imageRepo;
        private readonly IMapper _mapper;

        public GetEventImagesQueryHandler( IEventRepository eventRepo,IEventImageRepository imageRepo, IMapper mapper)
        {
            _eventRepo = eventRepo;
            _imageRepo = imageRepo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EventImageDto>> Handle( GetEventImagesQuery request,CancellationToken cancellationToken)
        {
            var evt = await _eventRepo
                .GetByIdAsync(request.EventId, cancellationToken)
                ?? throw new EntityNotFoundException(request.EventId);

            var images = await _imageRepo
                .ListByEventIdAsync(request.EventId, cancellationToken);

            return _mapper.Map<IEnumerable<EventImageDto>>(images);
        }
    }
}
