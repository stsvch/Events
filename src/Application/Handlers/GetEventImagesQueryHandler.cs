using Events.Application.Queries;
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
    public class GetEventImagesQueryHandler: IRequestHandler<GetEventImagesQuery, IEnumerable<string>>
    {
        private readonly IEventRepository _eventRepo;
        private readonly IEventImageRepository _imageRepo;

        public GetEventImagesQueryHandler( IEventRepository eventRepo,IEventImageRepository imageRepo)
        {
            _eventRepo = eventRepo;
            _imageRepo = imageRepo;
        }

        public async Task<IEnumerable<string>> Handle( GetEventImagesQuery request,CancellationToken cancellationToken)
        {
            var evt = await _eventRepo.GetByIdAsync(request.EventId, cancellationToken) ?? throw new EntityNotFoundException(request.EventId);

            var images = await _imageRepo.ListByEventIdAsync(request.EventId, cancellationToken);
            return images.Select(img => img.Url);
        }
    }
}
