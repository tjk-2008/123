using DirectoryService.Domain.Contracts;
using DirectoryService.Domain.PositionsContext;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Infrastructure.Repositories;

public class PositionRepository : IPositionRepository
{
	private readonly DirectoryDbContext _dbContext;

	public PositionRepository(DirectoryDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public async Task AddAsync(Position position, CancellationToken cancellationToken = default)
	{
		await _dbContext.Positions.AddAsync(position, cancellationToken);
		await _dbContext.SaveChangesAsync(cancellationToken);
	}

	public async Task<bool> IsNameUniqueAsync(string name, CancellationToken cancellationToken = default)
	{
		return !await _dbContext.Positions
			.AnyAsync(p => p.Name.Value == name, cancellationToken);
	}
}
