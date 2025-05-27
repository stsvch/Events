using Events.Application.Commands;
using Events.Application.DTOs;
using Events.Application.Interfaces;
using Events.Application.Queries;
using Events.WebApi.DTOs.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Events.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IImageStorageService _imageService;

        public EventsController(IMediator mediator, IImageStorageService imageService)
        {
            _mediator = mediator;
            _imageService = imageService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<PagedResultDto<EventDto>>> GetEvents(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] DateTimeOffset? startDate = null,
            [FromQuery] DateTimeOffset? endDate = null,
            [FromQuery] string? venue = null,
            [FromQuery] Guid? categoryId = null)
        {
            if (startDate.HasValue || endDate.HasValue || !string.IsNullOrWhiteSpace(venue) || categoryId.HasValue)
            {
                var searchQuery = new SearchEventsQuery
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    Venue = venue,
                    CategoryId = categoryId,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
                var filteredResult = await _mediator.Send(searchQuery);
                return Ok(filteredResult);
            }

            var query = new GetAllEventsQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                IncludeDetails = false
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<EventDetailDto>> GetEventDetails(Guid id)
        {
            var query = new GetEventDetailQuery(id);
            var eventDetail = await _mediator.Send(query);
            return eventDetail != null ? Ok(eventDetail) : NotFound();
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventRequest request)
        {
            var command = new CreateEventCommand
            {
                Title = request.Title,
                Date = request.Date,
                Venue = request.Venue,
                CategoryId = request.CategoryId,
                Description = request.Description,
                Capacity = request.Capacity
            };
            var newEventId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetEventDetails), new { id = newEventId });
        }

        [HttpGet("{id}/summary")]
        [AllowAnonymous]
        public async Task<ActionResult<EventDto>> GetEventSummary(Guid id)
        {
            var query = new GetEventSummaryQuery(id);
            var summary = await _mediator.Send(query);
            return summary != null ? Ok(summary) : NotFound();
        }


        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateEvent(Guid id, [FromBody] UpdateEventRequest request)
        {
            if (id != request.Id)
                return BadRequest("Mismatched Event ID");

            var command = new UpdateEventCommand
            {
                Id = request.Id,
                Title = request.Title,
                Date = request.Date,
                Venue = request.Venue,
                CategoryId = request.CategoryId,
                Description = request.Description,
                Capacity = request.Capacity
            };
            await _mediator.Send(command);
            await _mediator.Send(new NotifyParticipantsAboutChangeCommand
            {
                EventId = id,
                EventTitle = request.Title,
                Message = "Event data was updated."
            });
            return NoContent();
        }


        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteEvent(Guid id)
        {
            var command = new DeleteEventCommand { Id = id };
            await _mediator.Send(command);
            return NoContent();
        }

        [RequestSizeLimit(100_000_000)]
        [HttpPost("{id}/images")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UploadEventImage(Guid id, [FromForm] UploadEventImageRequest request)
        {
            if (request.File == null && string.IsNullOrWhiteSpace(request.Url))
                return BadRequest("Either file or Url must be provided.");

            string imageUrl;
            if (request.File != null)
            {
                using var stream = request.File.OpenReadStream();
                imageUrl = await _imageService.UploadAsync(stream, request.File.FileName);
            }
            else
            {
                imageUrl = request.Url;
            }

            await _mediator.Send(new AddEventImageCommand
            {
                EventId = id,
                Url = imageUrl
            });

            return Ok(new { url = imageUrl });
        }

    }
}