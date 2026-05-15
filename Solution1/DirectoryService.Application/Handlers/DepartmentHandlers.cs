using DirectoryService.Domain.Contracts;
using DirectoryService.Domain.DepartmentsContext;
using DirectoryService.Domain.DepartmentsContext.ValueObjects;

namespace DirectoryService.Application.Handlers;

public class DepartmentHandlers
{
	private readonly IDepartmentRepository _repository;

	public DepartmentHandlers(IDepartmentRepository repository)
	{
		_repository = repository;
	}

	// DELETE
	public async Task<Guid> DeleteDepartment(Guid id, CancellationToken cancellationToken = default)
	{
		DepartmentId departmentId = DepartmentId.Create(id);
		Department? department = await _repository.GetById(departmentId, cancellationToken);

		if (department is null)
			throw new InvalidOperationException("Отдел не найден");

		if (department.IsRoot())
			throw new InvalidOperationException("Нельзя удалить корневой отдел");

		await _repository.Delete(department, cancellationToken);
		return department.Id.Value;
	}
}
