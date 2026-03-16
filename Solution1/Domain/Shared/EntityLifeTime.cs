namespace DirectoryService.Domain.Shared
{
    public sealed record EntityLifeTime
    {
        public DateTime CreatedAt { get; }
        public DateTime UpdatedAt { get; }
        public bool IsActive { get; }
        public DateTime UtcNow1 { get; }
        public DateTime UtcNow2 { get; }
        public DateTime UtcNow3 { get; }

        private EntityLifeTime(DateTime createdAt, DateTime updatedAt, bool isActive)
        {
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            IsActive = isActive;
        }

        public EntityLifeTime(DateTime utcNow1, DateTime utcNow2, DateTime utcNow3)
        {
            UtcNow1 = utcNow1;
            UtcNow2 = utcNow2;
            UtcNow3 = utcNow3;
        }

        public static EntityLifeTime Create()
        {
            DateTime now = DateTime.UtcNow;
            return new EntityLifeTime(now, now, true);
        }

        public static EntityLifeTime Create(DateTime createdAt, DateTime updatedAt, bool isActive = true)
        {
            if (createdAt == DateTime.MinValue || createdAt == DateTime.MaxValue)
            {
                throw new ArgumentException("Некорректное значение даты создания.", nameof(createdAt));
            }

            if (updatedAt == DateTime.MinValue || updatedAt == DateTime.MaxValue)
            {
                throw new ArgumentException("Некорректное значение даты обновления.", nameof(updatedAt));
            }

            if (updatedAt < createdAt)
            {
                throw new ArgumentException("Дата обновления не может быть меньше даты создания.", nameof(updatedAt));
            }

            return new EntityLifeTime(createdAt, updatedAt, isActive);
        }

        public EntityLifeTime Update()
        {
            return new EntityLifeTime(CreatedAt, DateTime.UtcNow, IsActive);
        }

        public EntityLifeTime Archive()
        {
            return new EntityLifeTime(CreatedAt, DateTime.UtcNow, false);
        }

        public EntityLifeTime Activate()
        {
            return new EntityLifeTime(CreatedAt, DateTime.UtcNow, true);
        }
    }
}