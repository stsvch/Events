using Events.Application.Commands;
using Events.Application.DTOs;
using Events.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Events.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public CategoriesController(IMediator mediator) => _mediator = mediator;

        // GET api/categories
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll()
            => Ok(await _mediator.Send(new GetAllCategoriesQuery()));

        // GET api/categories/{id}
        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<ActionResult<CategoryDto>> GetById(Guid id)
            => Ok(await _mediator.Send(new GetCategoryByIdQuery(id)));

        // POST api/categories
        [HttpPost]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateCategoryCommand command)
        {
            var id = await _mediator.Send(command);
            // Возвращаем 201 Created с Location: api/categories/{id}
            return CreatedAtAction(nameof(GetById), new { id }, null);
        }

        // DELETE api/categories/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(new DeleteCategoryCommand { Id = id });
            return NoContent();
        }
    }
}
