using DirectoryService.Api.DTOs.Position;
using DirectoryService.Application.Handlers;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Api.Controllers;

[ApiController]
[Route("api/positions")]
public class PositionsController : ControllerBase
{
	private readonly PositionHandlers _handlers;

	public PositionsController(PositionHandlers handlers)
	{
		_handlers = handlers;
	}

	[HttpPost]
	public async Task<IActionResult> Create([FromBody] CreatePositionRequest request)
	{
		try
		{
			Guid id = await _handlers.CreatePosition(request.Name, request.Description);
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

			Guid updatedId = await _handlers.UpdatePosition(id, request.Name);
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
