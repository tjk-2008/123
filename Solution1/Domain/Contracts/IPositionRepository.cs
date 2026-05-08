using DirectoryService.Domain.PositionsContext;
using DirectoryService.Domain.PositionsContext.ValueObjects;

namespace DirectoryService.Domain.Contracts;

public interface IPositionRepository
{
    Task<Position?> GetById(Guid id, CancellationToken cancellationToken = default);
    Task<Position?> GetById(PositionId id, CancellationToken cancellationToken = default);
    Task<Position?> GetByName(PositionName name, CancellationToken cancellationToken = default);
    Task AddAsync(Position position, CancellationToken cancellationToken = default);
    Task Update(Position position, CancellationToken cancellationToken = default);
    Task<bool> IsNameUniqueAsync(string name, CancellationToken cancellationToken = default);
}