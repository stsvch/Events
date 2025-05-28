using Events.Application.Commands;
using Events.Application.DTOs;
using Events.Application.Interfaces;
using Events.Application.Queries;
using Events.WebApi.DTOs.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

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
                  [FromQuery] string? title = null,
                  [FromQuery] DateTimeOffset? startDate = null,
                  [FromQuery] DateTimeOffset? endDate = null,
                  [FromQuery] string? venue = null,
                  [FromQuery] Guid? categoryId = null,
                  [FromQuery] int pageNumber = 1,
                  [FromQuery] int pageSize = 10)
        {
            if (!string.IsNullOrWhiteSpace(title)
             || startDate.HasValue
             || endDate.HasValue
             || !string.IsNullOrWhiteSpace(venue)
             || categoryId.HasValue)
            {
                var searchQuery = new SearchEventsQuery
                {
                    Title = title,
                    StartDate = startDate,
                    EndDate = endDate,
                    Venue = venue,
                    CategoryId = categoryId,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
                var filtered = await _mediator.Send(searchQuery);
                return Ok(filtered);
            }

            var allQuery = new GetAllEventsQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            var result = await _mediator.Send(allQuery);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<ActionResult<EventDetailDto>> GetEventDetails(Guid id)
        {
            var dto = await _mediator.Send(new GetEventDetailQuery(id));
            return dto is null ? NotFound() : Ok(dto);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> CreateEvent([FromForm] CreateEventRequest request)
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

            return CreatedAtAction(
                nameof(GetEventDetails),
                new { id = newEventId },
                new { id = newEventId }
            );
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

    }
}