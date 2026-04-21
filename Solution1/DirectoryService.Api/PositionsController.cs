using DirectoryService.Domain.PositionsContext;
using DirectoryService.Domain.PositionsContext.ValueObjects;
using DirectoryService.Domain.Shared;
using DirectoryService.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Api;

[ApiController]
[Route("api/positions")]
public class PositionsController : ControllerBase
{
    private readonly DirectoryDbContext _dbContext;

    public PositionsController(DirectoryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        List<Position> positions = await _dbContext.Positions
            .Where(p => p.LifeTime.IsActive)
            .ToListAsync();

        IEnumerable<PositionResponse> response = positions.Select(p => new PositionResponse
        {
            Id = p.Id.Value,
            Name = p.Name.Value,
            Description = p.Description.Value,
            IsActive = p.LifeTime.IsActive,
            CreatedAt = p.LifeTime.CreatedAt,
            UpdatedAt = p.LifeTime.UpdatedAt
        });

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        PositionId positionId = PositionId.Create(id);
        Position? position = await _dbContext.Positions
            .FirstOrDefaultAsync(p => p.Id == positionId && p.LifeTime.IsActive);

        if (position == null)
        {
            return NotFound($"Должность с Id {id} не найдена");
        }

        return Ok(new PositionResponse
        {
            Id = position.Id.Value,
            Name = position.Name.Value,
            Description = position.Description.Value,
            IsActive = position.LifeTime.IsActive,
            CreatedAt = position.LifeTime.CreatedAt,
            UpdatedAt = position.LifeTime.UpdatedAt
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePositionRequest request)
    {
        try
        {
            Position position = new Position(
                PositionId.Create(),
                PositionName.Create(request.Name),
                PositionDescription.Create(request.Description),
                true,
                EntityLifeTime.Create()
            );

            await _dbContext.Positions.AddAsync(position);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = position.Id.Value }, new PositionResponse
            {
                Id = position.Id.Value,
                Name = position.Name.Value,
                Description = position.Description.Value,
                IsActive = position.LifeTime.IsActive,
                CreatedAt = position.LifeTime.CreatedAt,
                UpdatedAt = position.LifeTime.UpdatedAt
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch
        {
            return Conflict("Конфликт при создании должности");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePositionRequest request)
    {
        try
        {
            PositionId positionId = PositionId.Create(id);
            Position? position = await _dbContext.Positions
                .FirstOrDefaultAsync(p => p.Id == positionId && p.LifeTime.IsActive);

            if (position == null)
            {
                return NotFound($"Должность с Id {id} не найдена");
            }

            if (request.Name != null)
            {
                position.ChangePositionName(PositionName.Create(request.Name));
            }

            if (request.Description != null)
            {
                position.ChangeDescription(PositionDescription.Create(request.Description));
            }

            await _dbContext.SaveChangesAsync();

            return Ok(new PositionResponse
            {
                Id = position.Id.Value,
                Name = position.Name.Value,
                Description = position.Description.Value,
                IsActive = position.LifeTime.IsActive,
                CreatedAt = position.LifeTime.CreatedAt,
                UpdatedAt = position.LifeTime.UpdatedAt
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch
        {
            return Conflict("Конфликт при обновлении должности");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            PositionId positionId = PositionId.Create(id);
            Position? position = await _dbContext.Positions
                .FirstOrDefaultAsync(p => p.Id == positionId && p.LifeTime.IsActive);

            if (position == null)
            {
                return NotFound($"Должность с Id {id} не найдена");
            }

            position.ChangeActivity(false);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }
}