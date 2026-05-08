    using DirectoryService.Api.DTOs.Position;
using DirectoryService.Application.Commands.CreatePosition;
using DirectoryService.Application.Commands.UpdatePosition;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Api.Controllers;

[ApiController]
[Route("api/positions")]
public class PositionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PositionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePositionRequest request)
    {
        try
        {
            var command = new CreatePositionCommand(request.Name, request.Description);
            Guid id = await _mediator.Send(command);
            return CreatedAtAction(nameof(Create), new { id }, new { Id = id });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePositionRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest("Название должности не может быть пустым");
            }

            var command = new UpdatePositionCommand(id, request.Name);
            Guid updatedId = await _mediator.Send(command);
            return Ok(new { Id = updatedId });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}