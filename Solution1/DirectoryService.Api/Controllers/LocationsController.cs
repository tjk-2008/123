using DirectoryService.Api.DTOs.Location;
using DirectoryService.Application.Commands.CreateLocation;
using DirectoryService.Application.Commands.UpdateLocation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Api.Controllers;

[ApiController]
[Route("api/locations")]
public class LocationsController(IMediator mediator) : ControllerBase
{
	[HttpPost]
	public async Task<IActionResult> Create([FromBody] CreateLocationRequest request)
	{
		try
		{
			CreateLocationCommand command = new(request.Name, request.Address, request.TimeZone);
			Guid id = await mediator.Send(command);
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
			UpdateLocationCommand command = new(id, request.Name, request.Address, request.TimeZone);
			Guid updatedId = await mediator.Send(command);
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
