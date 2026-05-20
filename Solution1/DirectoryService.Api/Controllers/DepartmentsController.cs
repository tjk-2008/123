using DirectoryService.Application.Handlers;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Api.Controllers;

[ApiController]
[Route("api/departments")]
public class DepartmentsController : ControllerBase
{
	private readonly DepartmentHandlers _handlers;

	public DepartmentsController(DepartmentHandlers handlers)
	{
		_handlers = handlers;
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> Delete(Guid id)
	{
		try
		{
			Guid deletedId = await _handlers.DeleteDepartment(id);
			return Ok(new { Id = deletedId });
		}
		catch (InvalidOperationException ex)
		{
			return NotFound(ex.Message);
		}
	}
}
