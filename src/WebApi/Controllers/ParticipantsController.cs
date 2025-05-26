using AutoMapper;
using Events.Application.Commands;
using Events.Application.Queries;
using Events.WebApi.DTOs.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Events.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParticipantsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public ParticipantsController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        // GET: api/participants?eventId=...
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<ParticipantDto>>> GetParticipants([FromQuery] Guid? eventId)
        {
            IEnumerable<ParticipantDto> participants;
            if (eventId.HasValue)
            {
                // Получить участников конкретного события
                var query = new GetEventParticipantsQuery(eventId.Value);
                participants = await _mediator.Send(query);
            }
            else
            {
                // Получить всех участников системы
                participants = await _mediator.Send(new GetAllParticipantsQuery());
            }
            return Ok(participants);
        }

        // POST: api/participants/register
        [HttpPost("register")]
        [AllowAnonymous] // предполагаем, что регистрация на событие доступна без токена (для внешних пользователей)
        public async Task<IActionResult> RegisterParticipant([FromBody] RegisterParticipantRequest request)
        {
            var command = _mapper.Map<RegisterParticipantCommand>(request);
            await _mediator.Send(command);
            return Ok(new { message = "You have been registered for the event." });
        }

        // DELETE: api/participants/{participantId}?eventId=...
        [HttpDelete("{participantId}")]
        [Authorize]
        public async Task<IActionResult> UnregisterParticipant(Guid participantId, [FromQuery] Guid eventId)
        {
            // Удаление участника из события (отмена регистрации)
            var command = new UnregisterParticipantCommand { ParticipantId = participantId, EventId = eventId };
            await _mediator.Send(command);
            return NoContent();
        }
    }

}
