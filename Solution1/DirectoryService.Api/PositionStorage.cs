using DirectoryService.Domain.PositionsContext;
using DirectoryService.Domain.PositionsContext.ValueObjects;
using DirectoryService.Domain.Shared;

namespace DirectoryService.Api;

public class PositionStorage
{
    private static readonly Dictionary<PositionId, Position> _positions = new();

    public void Add(Position position)
    {
        if (_positions.ContainsKey(position.Id))
        {
            throw new ArgumentException("Должность с таким Id уже существует");
        }

        if (_positions.Values.Any(p => p.Name.Value == position.Name.Value))
        {
            throw new ArgumentException("Должность с таким названием уже существует");
        }

        _positions.Add(position.Id, position);
    }

    public Position? GetById(PositionId id)
    {
        if (!_positions.TryGetValue(id, out Position? position))
        {
            return null;
        }

        if (!position.LifeTime.IsActive)
        {
            return null;
        }

        return position;
    }

    public IEnumerable<Position> GetAll()
    {
        return _positions.Values.Where(p => p.LifeTime.IsActive).ToList();
    }

    public void Remove(PositionId id)
    {
        if (!_positions.TryGetValue(id, out Position? position))
        {
            throw new ArgumentException("Должность не найдена");
        }

        Position archived = new Position(
            position.Id,
            position.Name,
            position.Description,
            position.IsActive,
            position.LifeTime.Archive()
        );

        _positions[id] = archived;
    }

    public void Update(Position position)
    {
        if (!_positions.TryGetValue(position.Id, out Position? existing))
        {
            throw new ArgumentException("Должность не найдена");
        }

        if (!existing.LifeTime.IsActive)
        {
            throw new ArgumentException("Нельзя обновить архивированную должность");
        }

        if (_positions.Values.Any(p => p.Name.Value == position.Name.Value && p.Id != position.Id))
        {
            throw new ArgumentException("Должность с таким названием уже существует");
        }

        _positions[position.Id] = position;
    }

    public void InitializeStorage()
    {
        Position[] positions =
        [
            new Position(
                PositionId.Create(),
                PositionName.Create("Разработчик"),
                PositionDescription.Create("Разработка ПО"),
                true,
                EntityLifeTime.Create()
            ),
            new Position(
                PositionId.Create(),
                PositionName.Create("Тестировщик"),
                PositionDescription.Create("Тестирование ПО"),
                true,
                EntityLifeTime.Create()
            ),
            new Position(
                PositionId.Create(),
                PositionName.Create("Менеджер"),
                PositionDescription.Create("Управление проектами"),
                true,
                EntityLifeTime.Create()
            )
        ];

        foreach (Position position in positions)
        {
            _positions[position.Id] = position;
        }
    }
}