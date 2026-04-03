using DirectoryService.Domain.LocationsContext;
using DirectoryService.Domain.LocationsContext.ValueObjects;
using DirectoryService.Domain.Shared;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Api;

[ApiController]
[Route("api/locations")]
public class LocationsController : ControllerBase
{
    private readonly LocationStorage _storage;

    public LocationsController(LocationStorage storage)
    {
        _storage = storage;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        IEnumerable<Location> locations = _storage.GetAll();
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
    public IActionResult GetById(Guid id)
    {
        LocationId locationId = LocationId.Create(id);
        Location? location = _storage.GetById(locationId);

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
    public IActionResult Create([FromBody] CreateLocationRequest request)
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

            _storage.Add(location);

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
    public IActionResult Update(Guid id, [FromBody] UpdateLocationRequest request)
    {
        try
        {
            LocationId locationId = LocationId.Create(id);
            Location? existing = _storage.GetById(locationId);

            if (existing == null)
            {
                return NotFound($"Локация с Id {id} не найдена");
            }

            LocationName updatedName = request.Name != null ? LocationName.Create(request.Name) : existing.Name;
            LocationAddress updatedAddress = request.Address != null ? LocationAddress.Create(request.Address) : existing.Address;
            IanaTimeZone updatedTimeZone = request.TimeZone != null ? IanaTimeZone.Create(request.TimeZone) : existing.TimeZone;

            Location updated = new Location(
                existing.Id,
                updatedAddress,
                updatedName,
                updatedTimeZone,
                existing.LifeTime.Update()
            );

            _storage.Update(updated);

            return Ok(new LocationResponse
            {
                Id = updated.Id.Value,
                Name = updated.Name.Value,
                Address = updated.Address.Value,
                TimeZone = updated.TimeZone.Value,
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
            return Conflict("Конфликт при обновлении локации");
        }
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(Guid id)
    {
        try
        {
            LocationId locationId = LocationId.Create(id);
            Location? existing = _storage.GetById(locationId);

            if (existing == null)
            {
                return NotFound($"Локация с Id {id} не найдена");
            }

            _storage.Remove(locationId);

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }
}