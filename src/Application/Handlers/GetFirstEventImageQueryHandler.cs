﻿using Events.Application.Queries;
using Events.Domain.Exceptions;
using Events.Domain.Repositories;
using MediatR;

namespace Events.Application.Handlers
{
    public class GetFirstEventImageQueryHandler : IRequestHandler<GetFirstEventImageQuery, string?>
    {
        private readonly IEventRepository _eventRepo;
        private readonly IEventImageRepository _imageRepo;

        public GetFirstEventImageQueryHandler(IEventRepository eventRepo,IEventImageRepository imageRepo)
        {
            _eventRepo = eventRepo;
            _imageRepo = imageRepo;
        }

        public async Task<string?> Handle(
            GetFirstEventImageQuery request,
            CancellationToken cancellationToken)
        {
            var evt = await _eventRepo.GetByIdAsync(request.EventId, cancellationToken);

            var images = await _imageRepo.ListByEventIdAsync(request.EventId, cancellationToken);
            return images.OrderBy(img => img.UploadedAt)
                         .Select(img => img.Url)
                         .FirstOrDefault();
        }
    }
}
