using DirectoryService.Domain.Contracts;
using DirectoryService.Domain.PositionsContext;
using DirectoryService.Domain.PositionsContext.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Infrastructure.Repositories;

public class PositionRepository : IPositionRepository
{
    private readonly DirectoryDbContext _dbContext;

    public PositionRepository(DirectoryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Position?> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        PositionId positionId = PositionId.Create(id);
        return await GetById(positionId, cancellationToken);
    }

    public async Task<Position?> GetById(PositionId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Positions
            .FirstOrDefaultAsync(p => p.Id == id && p.LifeTime.IsActive, cancellationToken);
    }

    public async Task<Position?> GetByName(PositionName name, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Positions
            .FirstOrDefaultAsync(p => p.Name == name && p.LifeTime.IsActive, cancellationToken);
    }

    public async Task AddAsync(Position position, CancellationToken cancellationToken = default)
    {
        await _dbContext.Positions.AddAsync(position, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Update(Position position, CancellationToken cancellationToken = default)
    {
        _dbContext.Positions.Update(position);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> IsNameUniqueAsync(string name, CancellationToken cancellationToken = default)
    {
        return !await _dbContext.Positions.AnyAsync(p => p.Name.Value == name, cancellationToken);
    }
}