using AutoMapper;
using Events.WebApi.DTOs.Requests;
using Events.WebApi.DTOs.Responses;
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
        private readonly IMapper _mapper;

        public EventsController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        // GET: api/events?pageNumber=1&pageSize=10
        [HttpGet]
        [AllowAnonymous]  // доступно без авторизации
        public async Task<ActionResult<PagedResultDto<EventDto>>> GetEvents([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var query = new GetAllEventsQuery { PageNumber = pageNumber, PageSize = pageSize, IncludeDetails = false };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // GET: api/events/{id}
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<EventDetailDto>> GetEventDetails(Guid id)
        {
            var query = new GetEventDetailQuery(id);
            var eventDetail = await _mediator.Send(query);
            return eventDetail != null ? Ok(eventDetail) : NotFound();
        }

        // POST: api/events
        [HttpPost]
        [Authorize(Roles = "Admin")]  // только авторизованный пользователь (админ) может создавать события
        public async Task<ActionResult<EventDetailDto>> CreateEvent([FromBody] CreateEventRequest request)
        {
            // Валидация FluentValidation запускается автоматически (через pipeline) перед обработкой команды
            var command = _mapper.Map<CreateEventCommand>(request);
            var createdEvent = await _mediator.Send(command);
            // createdEvent можно вернуть как DTO. Предположим, команда возвращает EventDetailDto.
            return CreatedAtAction(nameof(GetEventDetails), new { id = createdEvent.Id }, createdEvent);
        }

        // PUT: api/events/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateEvent(Guid id, [FromBody] UpdateEventRequest request)
        {
            if (id != request.Id)
                return BadRequest("Mismatched Event ID");

            var command = _mapper.Map<UpdateEventCommand>(request);
            await _mediator.Send(command);
            return NoContent();
        }

        // DELETE: api/events/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteEvent(Guid id)
        {
            await _mediator.Send(new DeleteEventCommand { EventId = id });
            return NoContent();
        }
    }

}
