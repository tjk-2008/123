using DirectoryService.Domain.DepartmentsContext;
using DirectoryService.Domain.DepartmentsContext.ValueObjects;

namespace DirectoryService.Domain.Contracts;

public interface IDepartmentRepository
{
	Task<Department?> GetById(Guid id, CancellationToken cancellationToken = default);
	Task<Department?> GetById(DepartmentId id, CancellationToken cancellationToken = default);
	Task AddAsync(Department department, CancellationToken cancellationToken = default);
	Task Update(Department department, CancellationToken cancellationToken = default);
	Task Delete(Department department, CancellationToken cancellationToken = default);
}
