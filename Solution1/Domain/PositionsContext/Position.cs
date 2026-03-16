using DirectoryService.Domain.PositionsContext.ValueObjects;
using DirectoryService.Domain.Shared;

namespace DirectoryService.Domain.PositionsContext
{
    public class Position
    {
        public PositionId Id { get; }
        public PositionName Name { get; private set; }
        public PositionDescription Description { get; }
        public bool IsActive { get; }
        public EntityLifeTime LifeTime { get; set; }

        public Position(
            PositionId id,
            PositionName name,
            PositionDescription description,
            bool isActive,
            EntityLifeTime lifeTime
        )
        {
            Id = id;
            Name = name;
            Description = description;
            IsActive = isActive;
            LifeTime = lifeTime;
        }

        public void ChangePositionName(PositionName newname)
        {
            if (!LifeTime.IsActive)
            {
                throw new InvalidOperationException("Сущность удалена");
            }
            Name = newname;
            LifeTime = LifeTime.Update();
        }
        public void ChangePositionName(PositionName newName, Func<PositionName, bool> isNameUnique)
        {

            // Проверка уникальности названия (в рамках всей системы)
            if (!isNameUnique(newName))
            {
                throw new InvalidOperationException("Должность с таким названием уже существует в системе.");
            }

            Name = newName;
            LifeTime = LifeTime.Update();
        }
    }
}
