using DirectoryService.Domain.Contracts;
using DirectoryService.Domain.LocationsContext;
using DirectoryService.Domain.LocationsContext.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Infrastructure.Repositories;

public class LocationRepository : ILocationRepository
{
	private readonly DirectoryDbContext _dbContext;

	public LocationRepository(DirectoryDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public async Task<Location?> GetById(Guid id, CancellationToken cancellationToken = default)
	{
		LocationId locationId = LocationId.Create(id);
		return await GetById(locationId, cancellationToken);
	}

	public async Task<Location?> GetById(LocationId id, CancellationToken cancellationToken = default)
	{
		return await _dbContext.Locations.FirstOrDefaultAsync(
			l => l.Id == id && l.LifeTime.IsActive,
			cancellationToken
		);
	}

	public async Task<Location?> GetByName(LocationName name, CancellationToken cancellationToken = default)
	{
		return await _dbContext.Locations.FirstOrDefaultAsync(
			l => l.Name == name && l.LifeTime.IsActive,
			cancellationToken
		);
	}

	public async Task AddAsync(Location location, CancellationToken cancellationToken = default)
	{
		await _dbContext.Locations.AddAsync(location, cancellationToken);
		await _dbContext.SaveChangesAsync(cancellationToken);
	}

	public async Task Update(Location location, CancellationToken cancellationToken = default)
	{
		_dbContext.Locations.Update(location);
		await _dbContext.SaveChangesAsync(cancellationToken);
	}
}
