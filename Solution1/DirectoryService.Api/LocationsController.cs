using DirectoryService.Domain.LocationsContext;
using DirectoryService.Domain.LocationsContext.ValueObjects;
using DirectoryService.Domain.Shared;
using DirectoryService.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Api;

[ApiController]
[Route("api/locations")]
public class LocationsController : ControllerBase
{
    private readonly DirectoryDbContext _dbContext;

    public LocationsController(DirectoryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        List<Location> locations = await _dbContext.Locations
            .Where(l => l.LifeTime.IsActive)
            .ToListAsync();

        IEnumerable<LocationResponse> response = locations.Select(l => new LocationResponse
        {
            Id = l.Id.Value,
            Name = l.Name.Value,
            Address = l.Address.Value,
            TimeZone = l.TimeZone.Value,
            IsActive = l.LifeTime.IsActive,
            CreatedAt = l.LifeTime.CreatedAt,
            UpdatedAt = l.LifeTime.UpdatedAt
        });

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        LocationId locationId = LocationId.Create(id);
        Location? location = await _dbContext.Locations
            .FirstOrDefaultAsync(l => l.Id == locationId && l.LifeTime.IsActive);

        if (location == null)
        {
            return NotFound($"Локация с Id {id} не найдена");
        }

        return Ok(new LocationResponse
        {
            Id = location.Id.Value,
            Name = location.Name.Value,
            Address = location.Address.Value,
            TimeZone = location.TimeZone.Value,
            IsActive = location.LifeTime.IsActive,
            CreatedAt = location.LifeTime.CreatedAt,
            UpdatedAt = location.LifeTime.UpdatedAt
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLocationRequest request)
    {
        try
        {
            Location location = new Location(
                LocationId.Create(),
                LocationAddress.Create(request.Address),
                LocationName.Create(request.Name),
                IanaTimeZone.Create(request.TimeZone),
                EntityLifeTime.Create()
            );

            await _dbContext.Locations.AddAsync(location);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = location.Id.Value }, new LocationResponse
            {
                Id = location.Id.Value,
                Name = location.Name.Value,
                Address = location.Address.Value,
                TimeZone = location.TimeZone.Value,
                IsActive = location.LifeTime.IsActive,
                CreatedAt = location.LifeTime.CreatedAt,
                UpdatedAt = location.LifeTime.UpdatedAt
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch
        {
            return Conflict("Конфликт при создании локации");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateLocationRequest request)
    {
        try
        {
            LocationId locationId = LocationId.Create(id);
            Location? location = await _dbContext.Locations
                .FirstOrDefaultAsync(l => l.Id == locationId && l.LifeTime.IsActive);

            if (location == null)
            {
                return NotFound($"Локация с Id {id} не найдена");
            }

            if (request.Name != null)
            {
                location.ChangeName(LocationName.Create(request.Name));
            }

            if (request.Address != null)
            {
                location.ChangeAddress(LocationAddress.Create(request.Address));
            }

            if (request.TimeZone != null)
            {
                location.ChangeTimeZone(IanaTimeZone.Create(request.TimeZone));
            }

            await _dbContext.SaveChangesAsync();

            return Ok(new LocationResponse
            {
                Id = location.Id.Value,
                Name = location.Name.Value,
                Address = location.Address.Value,
                TimeZone = location.TimeZone.Value,
                IsActive = location.LifeTime.IsActive,
                CreatedAt = location.LifeTime.CreatedAt,
                UpdatedAt = location.LifeTime.UpdatedAt
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch
        {
            return Conflict("Конфликт при обновлении локации");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            LocationId locationId = LocationId.Create(id);
            Location? location = await _dbContext.Locations
                .FirstOrDefaultAsync(l => l.Id == locationId && l.LifeTime.IsActive);

            if (location == null)
            {
                return NotFound($"Локация с Id {id} не найдена");
            }

            location.ChangeActivity(false);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }
}