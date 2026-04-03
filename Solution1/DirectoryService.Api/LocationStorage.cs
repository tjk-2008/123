using DirectoryService.Domain.LocationsContext;
using DirectoryService.Domain.LocationsContext.ValueObjects;
using DirectoryService.Domain.Shared;

namespace DirectoryService.Api;

public class LocationStorage
{
    private static readonly Dictionary<LocationId, Location> _locations = new();

    public void Add(Location location)
    {
        if (_locations.ContainsKey(location.Id))
        {
            throw new ArgumentException("Локация с таким Id уже существует");
        }

        if (_locations.Values.Any(l => l.Name.Value == location.Name.Value))
        {
            throw new ArgumentException("Локация с таким названием уже существует");
        }

        _locations.Add(location.Id, location);
    }

    public Location? GetById(LocationId id)
    {
        if (!_locations.TryGetValue(id, out Location? location))
        {
            return null;
        }

        if (!location.LifeTime.IsActive)
        {
            return null;
        }

        return location;
    }

    public IEnumerable<Location> GetAll()
    {
        return _locations.Values.Where(l => l.LifeTime.IsActive).ToList();
    }

    public void Remove(LocationId id)
    {
        if (!_locations.TryGetValue(id, out Location? location))
        {
            throw new ArgumentException("Локация не найдена");
        }

        Location archived = new Location(
            location.Id,
            location.Address,
            location.Name,
            location.TimeZone,
            location.LifeTime.Archive()
        );

        _locations[id] = archived;
    }

    public void Update(Location location)
    {
        if (!_locations.TryGetValue(location.Id, out Location? existing))
        {
            throw new ArgumentException("Локация не найдена");
        }

        if (!existing.LifeTime.IsActive)
        {
            throw new ArgumentException("Нельзя обновить архивированную локацию");
        }

        if (_locations.Values.Any(l => l.Name.Value == location.Name.Value && l.Id != location.Id))
        {
            throw new ArgumentException("Локация с таким названием уже существует");
        }

        _locations[location.Id] = location;
    }

    public void InitializeStorage()
    {
        Location[] locations =
        [
            new Location(
                LocationId.Create(),
                LocationAddress.Create("ул. Ленина, 1"),
                LocationName.Create("Москва"),
                IanaTimeZone.Create("Europe/Moscow"),
                EntityLifeTime.Create()
            ),
            new Location(
                LocationId.Create(),
                LocationAddress.Create("Невский пр., 10"),
                LocationName.Create("Санкт-Петербург"),
                IanaTimeZone.Create("Europe/Moscow"),
                EntityLifeTime.Create()
            ),
            new Location(
                LocationId.Create(),
                LocationAddress.Create("ул. Тверская, 5"),
                LocationName.Create("Казань"),
                IanaTimeZone.Create("Europe/Moscow"),
                EntityLifeTime.Create()
            )
        ];

        foreach (Location? location in locations)
        {
            _locations[location.Id] = location;
        }
    }
}