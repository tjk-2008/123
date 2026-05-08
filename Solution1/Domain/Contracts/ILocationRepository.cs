using DirectoryService.Domain.LocationsContext;
using DirectoryService.Domain.LocationsContext.ValueObjects;

namespace DirectoryService.Domain.Contracts;

public interface ILocationRepository
{
    Task<Location?> GetById(Guid id, CancellationToken cancellationToken = default);
    Task<Location?> GetById(LocationId id, CancellationToken cancellationToken = default);
    Task<Location?> GetByName(LocationName name, CancellationToken cancellationToken = default);
    Task AddAsync(Location location, CancellationToken cancellationToken = default);
    Task Update(Location location, CancellationToken cancellationToken = default);
    Task<bool> IsNameUniqueAsync(string name, CancellationToken cancellationToken = default);
}