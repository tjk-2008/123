using DirectoryService.Domain.DepartmentsContext.ValueObjects;
using DirectoryService.Domain.LocationsContext;
using DirectoryService.Domain.LocationsContext.ValueObjects;
using DirectoryService.Domain.PositionsContext;
using DirectoryService.Domain.PositionsContext.ValueObjects;
using DirectoryService.Domain.Shared;

namespace DirectoryService.Domain.DepartmentsContext
{
    public class Department
    {
        public DepartmentId Id { get; }
        public DepartmentName Name { get; }
        public DepartmentIdentifier Identifier { get; }
        public DepartmentId? ParentId { get; }
        public DepartmentPath Path { get; }
        public DepartmentDepth Depth { get; }
        public bool IsActive { get; }
        public EntityLifeTime LifeTime { get; private set; }

        private readonly List<DepartmentPosition> _positions = [];
        private readonly List<DepartmentLocation> _locations = [];

        public IReadOnlyList<DepartmentPosition> Positions => _positions.AsReadOnly();
        public IReadOnlyList<DepartmentLocation> Locations => _locations.AsReadOnly();

        // Фабричный метод для создания корневого подразделения
        public static Department CreateRoot(DepartmentName name, DepartmentIdentifier identifier, bool isActive = true)
        {
            DepartmentId id = DepartmentId.Create();
            DepartmentPath path = DepartmentPath.CreateForRoot(identifier.Value);
            DepartmentDepth depth = DepartmentDepth.CalculateFromPath(path);
            EntityLifeTime lifeTime = EntityLifeTime.Create();

            return new Department(id, name, identifier, null, path, depth, isActive, lifeTime);
        }

        // Фабричный метод для создания дочернего подразделения
        public static Department CreateChild(
            DepartmentName name,
            DepartmentIdentifier identifier,
            Department parent,
            bool isActive = true
        )
        {
            DepartmentId id = DepartmentId.Create();
            DepartmentPath path = DepartmentPath.CreateForChild(parent.Path, identifier.Value);
            DepartmentDepth depth = DepartmentDepth.CalculateFromPath(path);
            EntityLifeTime lifeTime = EntityLifeTime.Create();

            return new Department(id, name, identifier, parent.Id, path, depth, isActive, lifeTime);
        }

        // Закрытый конструктор
        private Department(
            DepartmentId id,
            DepartmentName name,
            DepartmentIdentifier identifier,
            DepartmentId? parentId,
            DepartmentPath path,
            DepartmentDepth depth,
            bool isActive,
            EntityLifeTime lifeTime
        )
        {
            Id = id;
            Name = name;
            Identifier = identifier;
            ParentId = parentId;
            Path = path;
            Depth = depth;
            IsActive = isActive;
            LifeTime = lifeTime;
        }

        // Метод для изменения активности
        public Department ChangeActivity(bool isActive)
        {
            EntityLifeTime updatedLifeTime = EntityLifeTime.Create(
                createdAt: LifeTime.CreatedAt,
                updatedAt: DateTime.UtcNow,
                isActive: isActive
            );

            return new Department(Id, Name, Identifier, ParentId, Path, Depth, isActive, updatedLifeTime);
        }

        // Метод для проверки, является ли подразделение корневым
        public bool IsRoot()
        {
            return ParentId == null;
        }

        // Метод для проверки, является ли подразделение дочерним относительно другого
        public bool IsChildOf(Department parent)
        {
            if (parent == null)
            {
                return false;
            }

            return Path.Value.StartsWith(parent.Path.Value + ".", StringComparison.InvariantCultureIgnoreCase);
        }

        // ========== Методы для работы с должностями (связь многие ко многим через DepartmentPosition) ==========

        public void AddPosition(Position position, int rank)
        {
            ArgumentNullException.ThrowIfNull(position);

            if (_positions.Any(p => p.PositionId == position.Id))
            {
                throw new ArgumentException("Должность уже добавлена в подразделение");
            }

            if (_positions.Any(p => p.PositionRank.Value == rank))
            {
                throw new ArgumentException("Ранг уже используется в подразделении");
            }

            _positions.Add(new DepartmentPosition(Id, position.Id, rank));
            LifeTime = LifeTime.Update();
        }

        public void ChangePositionRank(PositionId positionId, int newRank)
        {
            DepartmentPosition? deptPosition = _positions.FirstOrDefault(p => p.PositionId == positionId);
            if (deptPosition == null)
            {
                throw new ArgumentException("Должность не найдена в подразделении");
            }

            if (_positions.Any(p => p.PositionRank.Value == newRank && p.PositionId != positionId))
            {
                throw new ArgumentException("Ранг уже используется");
            }

            deptPosition.ChangeRank(newRank);
            LifeTime = LifeTime.Update();
        }

        public void RemovePosition(PositionId positionId)
        {
            DepartmentPosition? deptPosition = _positions.FirstOrDefault(p => p.PositionId == positionId);
            if (deptPosition == null)
            {
                throw new ArgumentException("Должность не найдена в подразделении");
            }

            _positions.Remove(deptPosition);
            LifeTime = LifeTime.Update();
        }

        public IReadOnlyList<DepartmentPosition> GetPositions()
        {
            return _positions.AsReadOnly();
        }

        // ========== Методы для работы с офисами (связь многие ко многим через DepartmentLocation) ==========

        public void AddLocation(Location location)
        {
            ArgumentNullException.ThrowIfNull(location);

            if (_locations.Any(l => l.LocationId == location.Id))
            {
                throw new ArgumentException("Офис уже добавлен в подразделение");
            }

            _locations.Add(new DepartmentLocation(Id, location.Id));
            LifeTime = LifeTime.Update();
        }

        public void RemoveLocation(LocationId locationId)
        {
            DepartmentLocation? deptLocation = _locations.FirstOrDefault(l => l.LocationId == locationId);
            if (deptLocation == null)
            {
                throw new ArgumentException("Офис не найден в подразделении");
            }

            _locations.Remove(deptLocation);
            LifeTime = LifeTime.Update();
        }

        public IReadOnlyList<DepartmentLocation> GetLocations()
        {
            return _locations.AsReadOnly();
        }

        public bool HasLocation(LocationId locationId)
        {
            return _locations.Any(l => l.LocationId == locationId);
        }
    }
}