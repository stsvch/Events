using Events.Application.Commands;
using Events.Application.Interfaces;
using Events.Application.Queries;
using Events.WebApi.DTOs.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Events.WebApi.Controllers
{
    [ApiController]
    [Route("api/events/{eventId:guid}/images")]
    [Authorize(Policy = "AdminOnly")]
    public class EventImagesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IImageStorageService _imageService;

        public EventImagesController(
            IMediator mediator,
            IImageStorageService imageService)
        {
            _mediator = mediator;
            _imageService = imageService;
        }

        [HttpPost]
        [RequestSizeLimit(100_000_000)]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload(
            Guid eventId,
            [FromForm] UploadEventImageRequest request)
        {
            if (request.File == null && string.IsNullOrWhiteSpace(request.Url))
                return BadRequest("Either file or Url must be provided.");

            string imageUrl;
            if (request.File != null)
            {
                await using var stream = request.File.OpenReadStream();
                imageUrl = await _imageService.UploadAsync(
                    stream, request.File.FileName);
            }
            else
            {
                imageUrl = request.Url!.Trim();
            }

            await _mediator.Send(new AddEventImageCommand
            {
                EventId = eventId,
                Url = imageUrl
            });

            return Ok(new { url = imageUrl });
        }

        [HttpDelete("{imageId:guid}")]
        public async Task<IActionResult> Delete( Guid eventId, Guid imageId)
        {
            await _mediator.Send(new DeleteEventImageCommand
            {
                EventId = eventId,
                ImageId = imageId
            });

            return NoContent();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll(Guid eventId)
        {
            var dtos = await _mediator.Send(new GetEventImagesQuery { EventId = eventId });
            return Ok(dtos);
        }

        [HttpGet("first")]
        [AllowAnonymous]
        public async Task<IActionResult> GetFirst(Guid eventId)
        {
            var url = await _mediator.Send(new GetFirstEventImageQuery
            {
                EventId = eventId
            });

            if (url == null)
                return NotFound();

            return Ok(new { url });
        }
    }
}
