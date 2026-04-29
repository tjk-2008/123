using DirectoryService.Domain.Contracts;
using DirectoryService.Domain.LocationsContext;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Infrastructure.Repositories;

public class LocationRepository : ILocationRepository
{
	private readonly DirectoryDbContext _dbContext;

	public LocationRepository(DirectoryDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public async Task AddAsync(Location location, CancellationToken cancellationToken = default)
	{
		await _dbContext.Locations.AddAsync(location, cancellationToken);
		await _dbContext.SaveChangesAsync(cancellationToken);
	}

	public async Task<bool> IsNameUniqueAsync(string name, CancellationToken cancellationToken = default)
	{
		return !await _dbContext.Locations.AnyAsync(l => l.Name.Value == name, cancellationToken);
	}
}
