using DirectoryService.Api;
using DirectoryService.Domain.PositionsContext;
using DirectoryService.Domain.PositionsContext.ValueObjects;
using DirectoryService.Domain.Shared;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Api;

[ApiController]
[Route("api/positions")]
public class PositionsController : ControllerBase
{
    private readonly PositionStorage _storage;

    public PositionsController(PositionStorage storage)
    {
        _storage = storage;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        IEnumerable<Position> positions = _storage.GetAll();
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
    public IActionResult GetById(Guid id)
    {
        PositionId positionId = PositionId.Create(id);
        Position? position = _storage.GetById(positionId);

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
    public IActionResult Create([FromBody] CreatePositionRequest request)
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

            _storage.Add(position);

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
    public IActionResult Update(Guid id, [FromBody] UpdatePositionRequest request)
    {
        try
        {
            PositionId positionId = PositionId.Create(id);
            Position? existing = _storage.GetById(positionId);

            if (existing == null)
            {
                return NotFound($"Должность с Id {id} не найдена");
            }

            PositionName updatedName = request.Name != null ? PositionName.Create(request.Name) : existing.Name;
            PositionDescription updatedDescription = request.Description != null ? PositionDescription.Create(request.Description) : existing.Description;

            Position updated = new Position(
                existing.Id,
                updatedName,
                updatedDescription,
                existing.IsActive,
                existing.LifeTime.Update()
            );

            _storage.Update(updated);

            return Ok(new PositionResponse
            {
                Id = updated.Id.Value,
                Name = updated.Name.Value,
                Description = updated.Description.Value,
                IsActive = updated.LifeTime.IsActive,
                CreatedAt = updated.LifeTime.CreatedAt,
                UpdatedAt = updated.LifeTime.UpdatedAt
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
    public IActionResult Delete(Guid id)
    {
        try
        {
            PositionId positionId = PositionId.Create(id);
            Position? existing = _storage.GetById(positionId);

            if (existing == null)
            {
                return NotFound($"Должность с Id {id} не найдена");
            }

            _storage.Remove(positionId);

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }
}