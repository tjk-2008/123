using DirectoryService.Domain.DepartmentsContext.ValueObjects;
using DirectoryService.Domain.LocationsContext.ValueObjects;
using DirectoryService.Domain.PositionsContext.ValueObjects;
using DirectoryService.Domain.Shared;
using Domain.LocationsContext.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DirectoryService.Domain.DepartmentsContext
{
    /// <summary>
    /// Сущность подразделения.
    /// </summary>
    public class Department
    {
        private readonly List<DepartmentPosition> _positions = new();
        private readonly List<DepartmentLocation> _locations = new();
        private readonly List<DepartmentId> _childDepartmentIds = new();

        /// <summary>
        /// Получает идентификатор подразделения.
        /// </summary>
        public DepartmentId Id { get; private set; }

        /// <summary>
        /// Получает название подразделения.
        /// </summary>
        public DepartmentName Name { get; private set; }

        /// <summary>
        /// Получает строковый идентификатор подразделения (slug).
        /// </summary>
        public DepartmentIdentifier Identifier { get; private set; }

        /// <summary>
        /// Получает идентификатор родительского подразделения.
        /// </summary>
        public DepartmentId? ParentId { get; private set; }

        /// <summary>
        /// Получает путь подразделения в иерархии.
        /// </summary>
        public DepartmentPath Path { get; private set; }

        /// <summary>
        /// Получает глубину подразделения в иерархии.
        /// </summary>
        public DepartmentDepth Depth { get; private set; }

        /// <summary>
        /// Получает жизненный цикл сущности.
        /// </summary>
        public EntityLifeTime LifeTime { get; private set; }

        /// <summary>
        /// Получает должности в подразделении.
        /// </summary>
        public IReadOnlyCollection<DepartmentPosition> Positions => _positions.AsReadOnly();

        /// <summary>
        /// Получает локации подразделения.
        /// </summary>
        public IReadOnlyCollection<DepartmentLocation> Locations => _locations.AsReadOnly();

        /// <summary>
        /// Получает идентификаторы дочерних подразделений.
        /// </summary>
        public IReadOnlyCollection<DepartmentId> ChildDepartmentIds => _childDepartmentIds.AsReadOnly();

        /// <summary>
        /// Проверяет, является ли подразделение активным.
        /// </summary>
        public bool IsActive => LifeTime.IsActive;

        /// <summary>
        /// Проверяет, является ли подразделение архивированным.
        /// </summary>
        public bool IsArchived => !LifeTime.IsActive;

        /// <summary>
        /// Фабричный метод для создания корневого подразделения.
        /// </summary>
        /// <param name="name">Название подразделения.</param>
        /// <param name="identifier">Идентификатор подразделения.</param>
        /// <param name="existingDepartments">Существующие подразделения для проверки уникальности названия.</param>
        /// <param name="isActive">Состояние активности.</param>
        /// <returns>Новое корневое подразделение.</returns>
        public static Department CreateRoot(
            DepartmentName name,
            DepartmentIdentifier identifier,
            IEnumerable<Department> existingDepartments,
            bool isActive = true)
        {
            ArgumentNullException.ThrowIfNull(name);
            ArgumentNullException.ThrowIfNull(identifier);
            ArgumentNullException.ThrowIfNull(existingDepartments);

            // Проверка уникальности названия в системе (только среди неархивированных)
            if (existingDepartments.Any(d => !d.IsArchived && d.Name.EqualsCaseInsensitive(name)))
            {
                throw new InvalidOperationException(
                    $"Подразделение с названием '{name}' уже существует.");
            }

            var id = DepartmentId.Create();
            var path = DepartmentPath.CreateRoot(identifier);
            var depth = DepartmentDepth.Create(0);
            var lifeTime = EntityLifeTime.CreateNew(isActive);

            return new Department(id, name, identifier, null, path, depth, lifeTime);
        }

        /// <summary>
        /// Фабричный метод для создания дочернего подразделения.
        /// </summary>
        /// <param name="name">Название подразделения.</param>
        /// <param name="identifier">Идентификатор подразделения.</param>
        /// <param name="parent">Родительское подразделение.</param>
        /// <param name="existingDepartments">Существующие подразделения для проверки уникальности.</param>
        /// <param name="isActive">Состояние активности.</param>
        /// <returns>Новое дочернее подразделение.</returns>
        public static Department CreateChild(
            DepartmentName name,
            DepartmentIdentifier identifier,
            Department parent,
            IEnumerable<Department> existingDepartments,
            bool isActive = true)
        {
            ArgumentNullException.ThrowIfNull(name);
            ArgumentNullException.ThrowIfNull(identifier);
            ArgumentNullException.ThrowIfNull(parent);
            ArgumentNullException.ThrowIfNull(existingDepartments);

            if (parent.IsArchived)
            {
                throw new InvalidOperationException("Нельзя создать подразделение под архивированным родителем.");
            }

            if (parent.Depth.Value >= DepartmentDepth.MaxDepth)
            {
                throw new InvalidOperationException(
                    $"Достигнута максимальная глубина иерархии ({DepartmentDepth.MaxDepth}).");
            }

            // Проверка уникальности названия в системе
            if (existingDepartments.Any(d => !d.IsArchived && d.Name.EqualsCaseInsensitive(name)))
            {
                throw new InvalidOperationException(
                    $"Подразделение с названием '{name}' уже существует.");
            }

            var id = DepartmentId.Create();
            var path = DepartmentPath.CreateChild(parent.Path, identifier);
            var depth = parent.Depth.Increment();
            var lifeTime = EntityLifeTime.CreateNew(isActive);

            var child = new Department(id, name, identifier, parent.Id, path, depth, lifeTime);

            // Добавляем ребенка к родителю
            parent.AddChildDepartmentId(id);

            return child;
        }

        private Department(
            DepartmentId id,
            DepartmentName name,
            DepartmentIdentifier identifier,
            DepartmentId? parentId,
            DepartmentPath path,
            DepartmentDepth depth,
            EntityLifeTime lifeTime)
        {
            Id = id;
            Name = name;
            Identifier = identifier;
            ParentId = parentId;
            Path = path;
            Depth = depth;
            LifeTime = lifeTime;
        }

        /// <summary>
        /// Изменяет активность подразделения.
        /// </summary>
        /// <param name="isActive">Новое состояние активности.</param>
        /// <returns>Обновленное подразделение.</returns>
        public Department ChangeActivity(bool isActive)
        {
            if (IsArchived)
            {
                throw new InvalidOperationException("Нельзя изменить активность архивированного подразделения.");
            }

            var updatedLifeTime = LifeTime.Update(isActive);

            return new Department(Id, Name, Identifier, ParentId, Path, Depth, updatedLifeTime);
        }

        /// <summary>
        /// Добавляет должность в подразделение.
        /// </summary>
        /// <param name="positionId">Идентификатор должности.</param>
        /// <param name="positionName">Название должности.</param>
        public void AddPosition(PositionId positionId, PositionName positionName)
        {
            if (IsArchived)
            {
                throw new InvalidOperationException("Нельзя добавить должность в архивированное подразделение.");
            }

            ArgumentNullException.ThrowIfNull(positionId);
            ArgumentNullException.ThrowIfNull(positionName);

            // Проверка уникальности по ID внутри подразделения
            if (_positions.Any(p => p.PositionId == positionId))
            {
                throw new InvalidOperationException(
                    $"Должность с ID '{positionId.Value}' уже существует в подразделении.");
            }

            // Проверка уникальности по названию внутри подразделения
            if (_positions.Any(p => p.PositionName.EqualsCaseInsensitive(positionName)))
            {
                throw new InvalidOperationException(
                    $"Должность с названием '{positionName.Value}' уже существует в подразделении.");
            }

            _positions.Add(new DepartmentPosition(positionId, positionName, DateTime.UtcNow));
            UpdateLifeTime();
        }

        /// <summary>
        /// Удаляет должность из подразделения.
        /// </summary>
        /// <param name="positionId">Идентификатор должности.</param>
        public void RemovePosition(PositionId positionId)
        {
            if (IsArchived)
            {
                throw new InvalidOperationException("Нельзя удалить должность из архивированного подразделения.");
            }

            ArgumentNullException.ThrowIfNull(positionId);

            var position = _positions.FirstOrDefault(p => p.PositionId == positionId);
            if (position is null)
            {
                throw new InvalidOperationException(
                    $"Должность с ID '{positionId.Value}' не найдена в подразделении.");
            }

            _positions.Remove(position);
            UpdateLifeTime();
        }

        /// <summary>
        /// Добавляет локацию в подразделение.
        /// </summary>
        /// <param name="locationId">Идентификатор локации.</param>
        /// <param name="locationAddress">Адрес локации.</param>
        public void AddLocation(LocationId locationId, LocationAddress locationAddress)
        {
            if (IsArchived)
            {
                throw new InvalidOperationException("Нельзя добавить локацию в архивированное подразделение.");
            }

            ArgumentNullException.ThrowIfNull(locationId);
            ArgumentNullException.ThrowIfNull(locationAddress);

            // Проверка уникальности по ID внутри подразделения
            if (_locations.Any(l => l.LocationId == locationId))
            {
                throw new InvalidOperationException(
                    $"Локация с ID '{locationId.Value}' уже существует в подразделении.");
            }

            // Проверка уникальности по адресу внутри подразделения
            if (_locations.Any(l => l.LocationAddress.Equals(locationAddress)))
            {
                throw new InvalidOperationException(
                    $"Локация с адресом '{locationAddress.Value}' уже существует в подразделении.");
            }

            _locations.Add(new DepartmentLocation(locationId, locationAddress, DateTime.UtcNow));
            UpdateLifeTime();
        }

        /// <summary>
        /// Удаляет локацию из подразделения.
        /// </summary>
        /// <param name="locationId">Идентификатор локации.</param>
        public void RemoveLocation(LocationId locationId)
        {
            if (IsArchived)
            {
                throw new InvalidOperationException("Нельзя удалить локацию из архивированного подразделения.");
            }

            ArgumentNullException.ThrowIfNull(locationId);

            var location = _locations.FirstOrDefault(l => l.LocationId == locationId);
            if (location is null)
            {
                throw new InvalidOperationException(
                    $"Локация с ID '{locationId.Value}' не найдена в подразделении.");
            }

            _locations.Remove(location);
            UpdateLifeTime();
        }

        /// <summary>
        /// Устанавливает или изменяет родительское подразделение.
        /// </summary>
        /// <param name="newParentId">Идентификатор нового родительского подразделения.</param>
        /// <param name="newParent">Новое родительское подразделение.</param>
        public void SetParent(DepartmentId? newParentId, Department newParent)
        {
            if (IsArchived)
            {
                throw new InvalidOperationException("Нельзя изменить родителя архивированного подразделения.");
            }

            // Проверка на присоединение к самому себе
            if (newParentId?.Value == Id.Value)
            {
                throw new InvalidOperationException("Подразделение не может быть родителем самого себя.");
            }

            // Проверка на повторное присоединение к тому же родителю
            if (ParentId?.Value == newParentId?.Value)
            {
                throw new InvalidOperationException("Подразделение уже присоединено к этому родителю.");
            }

            if (newParentId.HasValue)
            {
                ArgumentNullException.ThrowIfNull(newParent);

                if (newParent.IsArchived)
                {
                    throw new InvalidOperationException("Нельзя установить архивированное подразделение как родителя.");
                }

                // Проверка на циклическую зависимость
                if (newParent.IsDescendantOf(this))
                {
                    throw new InvalidOperationException("Обнаружена циклическая зависимость в иерархии.");
                }

                // Проверка максимальной глубины
                if (newParent.Depth.Value >= DepartmentDepth.MaxDepth)
                {
                    throw new InvalidOperationException(
                        $"Родитель достиг максимальной глубины ({DepartmentDepth.MaxDepth}).");
                }

                // Обновляем путь и глубину
                var newPath = DepartmentPath.CreateChild(newParent.Path, Identifier);
                var newDepth = newParent.Depth.Increment();

                ParentId = newParentId;
                Path = newPath;
                Depth = newDepth;

                newParent.AddChildDepartmentId(Id);
            }
            else
            {
                // Делаем корневым
                ParentId = null;
                Path = DepartmentPath.CreateRoot(Identifier);
                Depth = DepartmentDepth.Create(0);
            }

            UpdateLifeTime();
        }

        /// <summary>
        /// Архивирует подразделение.
        /// </summary>
        public void Archive()
        {
            if (IsArchived)
            {
                throw new InvalidOperationException("Подразделение уже архивировано.");
            }

            LifeTime = LifeTime.Update(isActive: false);
        }

        /// <summary>
        /// Разархивирует подразделение.
        /// </summary>
        public void Unarchive()
        {
            if (!IsArchived)
            {
                throw new InvalidOperationException("Подразделение не архивировано.");
            }

            LifeTime = LifeTime.Update(isActive: true);
        }

        /// <summary>
        /// Проверяет, является ли подразделение корневым.
        /// </summary>
        public bool IsRoot() => ParentId is null;

        /// <summary>
        /// Проверяет, является ли подразделение потомком другого подразделения.
        /// </summary>
        /// <param name="potentialAncestor">Потенциальный предок.</param>
        public bool IsDescendantOf(Department potentialAncestor)
        {
            if (potentialAncestor is null)
            {
                return false;
            }

            // Проверяем прямую связь
            if (ParentId?.Value == potentialAncestor.Id.Value)
            {
                return true;
            }

            // Проверяем через путь
            return Path.IsDescendantOf(potentialAncestor.Path);
        }

        /// <summary>
        /// Проверяет, является ли подразделение предком другого подразделения.
        /// </summary>
        /// <param name="potentialDescendant">Потенциальный потомок.</param>
        public bool IsAncestorOf(Department potentialDescendant)
        {
            return potentialDescendant?.IsDescendantOf(this) ?? false;
        }

        /// <summary>
        /// Проверяет, содержит ли подразделение указанную должность.
        /// </summary>
        /// <param name="positionId">Идентификатор должности.</param>
        public bool ContainsPosition(PositionId positionId)
        {
            return positionId is not null && _positions.Any(p => p.PositionId == positionId);
        }

        /// <summary>
        /// Проверяет, содержит ли подразделение указанную локацию.
        /// </summary>
        /// <param name="locationId">Идентификатор локации.</param>
        public bool ContainsLocation(LocationId locationId)
        {
            return locationId is not null && _locations.Any(l => l.LocationId == locationId);
        }

        private void UpdateLifeTime()
        {
            LifeTime = LifeTime.Update();
        }

        internal void AddChildDepartmentId(DepartmentId childId)
        {
            ArgumentNullException.ThrowIfNull(childId);

            if (!_childDepartmentIds.Contains(childId))
            {
                _childDepartmentIds.Add(childId);
            }
        }

        internal void RemoveChildDepartmentId(DepartmentId childId)
        {
            ArgumentNullException.ThrowIfNull(childId);
            _childDepartmentIds.Remove(childId);
        }
    }

    /// <summary>
    /// Сущность-связка для связи подразделения и должности (многие-ко-многим).
    /// </summary>
    public sealed record DepartmentPosition(
        PositionId PositionId,
        PositionName PositionName,
        DateTime AssignedAt);

    /// <summary>
    /// Сущность-связка для связи подразделения и локации (многие-ко-многим).
    /// </summary>
    public sealed record DepartmentLocation(
        LocationId LocationId,
        LocationAddress LocationAddress,
        DateTime AssignedAt);
}