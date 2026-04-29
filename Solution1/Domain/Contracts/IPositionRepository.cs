using DirectoryService.Domain.PositionsContext;
using DirectoryService.Domain.PositionsContext.ValueObjects;
using DirectoryService.Domain.Shared;

namespace DirectoryService.Domain.Contracts;

public interface IPositionRepository
{
	Task AddAsync(Position position, CancellationToken cancellationToken = default);
	Task<bool> IsNameUniqueAsync(string name, CancellationToken cancellationToken = default);
}
