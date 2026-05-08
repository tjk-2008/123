using DirectoryService.Api.DTOs.Location;
using DirectoryService.Application.Commands.CreateLocation;
using DirectoryService.Application.Commands.UpdateLocation;
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
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateLocationRequest request)
    {
        try
        {
            var command = new UpdateLocationCommand(id, request.Name, request.Address, request.TimeZone);
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