using DirectoryService.Domain.LocationsContext;

namespace DirectoryService.Domain.Contracts;

public interface ILocationRepository
{
	Task AddAsync(Location location, CancellationToken cancellationToken = default);
	Task<bool> IsNameUniqueAsync(string name, CancellationToken cancellationToken = default);
}
