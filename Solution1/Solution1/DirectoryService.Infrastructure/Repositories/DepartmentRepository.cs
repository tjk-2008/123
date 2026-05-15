using DirectoryService.Domain.Contracts;
using DirectoryService.Domain.DepartmentsContext;
using DirectoryService.Domain.DepartmentsContext.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Infrastructure.Repositories;

public class DepartmentRepository : IDepartmentRepository
{
	private readonly DirectoryDbContext _dbContext;

	public DepartmentRepository(DirectoryDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public async Task<Department?> GetById(Guid id, CancellationToken cancellationToken = default)
	{
		DepartmentId departmentId = DepartmentId.Create(id);
		return await GetById(departmentId, cancellationToken);
	}

	public async Task<Department?> GetById(DepartmentId id, CancellationToken cancellationToken = default)
	{
		return await _dbContext.Departments.FirstOrDefaultAsync(
			d => d.Id == id && d.LifeTime.IsActive,
			cancellationToken
		);
	}

	public async Task AddAsync(Department department, CancellationToken cancellationToken = default)
	{
		await _dbContext.Departments.AddAsync(department, cancellationToken);
		await _dbContext.SaveChangesAsync(cancellationToken);
	}

	public async Task Update(Department department, CancellationToken cancellationToken = default)
	{
		_dbContext.Departments.Update(department);
		await _dbContext.SaveChangesAsync(cancellationToken);
	}

	public async Task Delete(Department department, CancellationToken cancellationToken = default)
	{
		_dbContext.Departments.Remove(department);
		await _dbContext.SaveChangesAsync(cancellationToken);
	}
}
