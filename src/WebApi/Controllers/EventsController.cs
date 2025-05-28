using Events.Application.Commands;
using Events.Application.DTOs;
using Events.Application.Interfaces;
using Events.Application.Queries;
using Events.Domain.Common;
using Events.Domain.Exceptions;
using Events.WebApi.DTOs.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.Security.Claims;

namespace Events.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EventsController(IMediator mediator)
        {
            _mediator = mediator;
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
            [FromQuery] int pageSize = 10,
            [FromQuery] SpecificationCombineMode combineMode = SpecificationCombineMode.And)
        {
            bool hasFilters = !string.IsNullOrWhiteSpace(title)
                           || startDate.HasValue
                           || endDate.HasValue
                           || !string.IsNullOrWhiteSpace(venue)
                           || categoryId.HasValue;

            if (hasFilters)
            {
                var searchQuery = new SearchEventsQuery
                {
                    Title = title,
                    StartDate = startDate,
                    EndDate = endDate,
                    Venue = venue,
                    CategoryId = categoryId,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    CombineMode = combineMode
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
            try
            {
                var dto = await _mediator.Send(new GetEventDetailQuery(id));
                return Ok(dto);
            }
            catch (EntityNotFoundException)
            {
                return NotFound();
            }
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

        [HttpGet("me")]
        [Authorize(Policy = "RegisteredUser")]
        public async Task<ActionResult<IEnumerable<EventDto>>> GetMyEvents()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var query = new GetEventsByParticipantQuery(userId);
            var events = await _mediator.Send(query);
            return Ok(events);
        }

    }
}