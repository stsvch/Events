using AutoMapper;
using Events.Application.Commands;
using Events.Application.DTOs;
using Events.Application.Queries;
using Events.WebApi.DTOs.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Events.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ParticipantsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ParticipantsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        // GET: api/participants?eventId=...
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<ParticipantDto>>> GetParticipants([FromQuery] Guid? eventId)
        {
            if (eventId.HasValue)
            {
                var participants = await _mediator.Send(new GetEventParticipantsQuery { EventId = eventId.Value });
                return Ok(participants);
            }

            // Для получения всех участников используем новый запрос GetAllParticipantsQuery
            var allParticipants = await _mediator.Send(new GetAllParticipantsQuery());
            return Ok(allParticipants);
        }

        // POST: api/participants/register
        [HttpPost("register")]
        public async Task<IActionResult> RegisterParticipant([FromBody] RegisterParticipantRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
          ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var command = new RegisterParticipantCommand
            {
                EventId = request.EventId,
                FullName = request.FullName,
                Email = request.Email,
                DateOfBirth = request.DateOfBirth
            };
            await _mediator.Send(command);
            return Ok(new { message = "You have been registered for the event." });
        }

        // DELETE: api/participants/{participantId}?eventId=...
        [HttpDelete("{participantId}")]
        public async Task<IActionResult> UnregisterParticipant(Guid participantId, [FromQuery] Guid eventId)
        {
            var command = new UnregisterParticipantCommand
            {
                ParticipantId = participantId,
                EventId = eventId
            };
            await _mediator.Send(command);
            return NoContent();
        }
    }
}