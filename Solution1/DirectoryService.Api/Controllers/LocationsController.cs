using DirectoryService.Api.DTOs.Location;
using DirectoryService.Application.Commands.CreateLocation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Api.Controllers;

[ApiController]
[Route("api/locations")]
public class LocationsController : ControllerBase
{
	private readonly IMediator _mediator;

	public LocationsController(IMediator mediator)
	{
		_mediator = mediator;
	}

	[HttpPost]
	public async Task<IActionResult> Create([FromBody] CreateLocationRequest request)
	{
		try
		{
			var command = new CreateLocationCommand(request.Name, request.Address, request.TimeZone);
			var id = await _mediator.Send(command);
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
}
