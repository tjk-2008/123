using DirectoryService.Api.DTOs.Location;
using DirectoryService.Application.Handlers;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Api.Controllers;

[ApiController]
[Route("api/locations")]
public class LocationsController : ControllerBase
{
	private readonly LocationHandlers _handlers;

	public LocationsController(LocationHandlers handlers)
	{
		_handlers = handlers;
	}

	[HttpPost]
	public async Task<IActionResult> Create([FromBody] CreateLocationRequest request)
	{
		try
		{
			Guid id = await _handlers.CreateLocation(request.Name, request.Address, request.TimeZone);
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
	public async Task<IActionResult> Update(Guid id, [FromBody] UpdateLocationRequest request)
	{
		try
		{
			Guid updatedId = await _handlers.UpdateLocation(id, request.Name, request.Address, request.TimeZone);
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

	[HttpDelete("{id}")]
	public async Task<IActionResult> Delete(Guid id)
	{
		try
		{
			Guid deletedId = await _handlers.DeleteLocation(id);
			return Ok(new { Id = deletedId });
		}
		catch (InvalidOperationException ex)
		{
			return NotFound(ex.Message);
		}
	}
}
