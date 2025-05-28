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
    [Authorize(Policy = "RegisteredUser")]
    public class ParticipantsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ParticipantsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ParticipantDto>>> GetParticipants([FromQuery] Guid? eventId)
        {
            if (eventId.HasValue)
            {
                var participants = await _mediator.Send(new GetEventParticipantsQuery { EventId = eventId.Value });
                return Ok(participants);
            }

            var allParticipants = await _mediator.Send(new GetAllParticipantsQuery());
            return Ok(allParticipants);
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterParticipant(
            [FromBody] RegisterParticipantRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            await _mediator.Send(new RegisterParticipantCommand
            {
                EventId = request.EventId,
                UserId = userId
            });

            return Ok(new { message = "You have been registered for the event." });
        }


        [HttpPost("unregister")]
        public async Task<IActionResult> UnregisterParticipant(
            [FromBody] UnregisterParticipantRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            await _mediator.Send(new UnregisterParticipantCommand
            {
                EventId = request.EventId,
                UserId = userId
            });

            return Ok(new { message = "You have been unregistered from the event." });
        }


        [HttpGet("is-registered")]
        public async Task<ActionResult<RegistrationStatusDto>> IsRegistered([FromQuery] Guid eventId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var status = await _mediator.Send(new CheckUserRegistrationQuery
            {
                EventId = eventId,
                UserId = userId
            });

            return Ok(status);
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<ActionResult<ParticipantDto>> GetById(Guid id)
        {
            var dto = await _mediator.Send(new GetParticipantByIdQuery { ParticipantId = id });
            if (dto is null) return NotFound();
            return Ok(dto);
        }

        [HttpGet("me")]
        public async Task<ActionResult<ParticipantDto>> GetCurrentUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var dto = await _mediator.Send(new GetParticipantByUserIdQuery { UserId = userId });
            if (dto is null) return NotFound();
            return Ok(dto);
        }
    }
}