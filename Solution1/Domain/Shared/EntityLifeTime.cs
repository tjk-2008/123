using System;

namespace DirectoryService.Domain.Shared
{
    /// <summary>
    /// Жизненный цикл сущности (время создания, обновления, состояние активности).
    /// </summary>
    public sealed record EntityLifeTime
    {
        /// <summary>
        /// Дата и время создания сущности.
        /// </summary>
        public DateTime CreatedAt { get; }

        /// <summary>
        /// Дата и время последнего обновления сущности.
        /// </summary>
        public DateTime UpdatedAt { get; }

        /// <summary>
        /// Флаг активности сущности.
        /// </summary>
        public bool IsActive { get; }

        private EntityLifeTime(DateTime createdAt, DateTime updatedAt, bool isActive)
        {
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            IsActive = isActive;
        }

        /// <summary>
        /// Создает новый жизненный цикл сущности.
        /// </summary>
        /// <param name="createdAt">Дата создания.</param>
        /// <param name="updatedAt">Дата обновления.</param>
        /// <param name="isActive">Состояние активности.</param>
        /// <returns>Новый экземпляр <see cref="EntityLifeTime"/>.</returns>
        public static EntityLifeTime Create(DateTime createdAt, DateTime updatedAt, bool isActive = true)
        {
            ValidateDateTime(createdAt, nameof(createdAt), "создания");
            ValidateDateTime(updatedAt, nameof(updatedAt), "обновления");

            if (updatedAt < createdAt)
            {
                throw new ArgumentException(
                    "Дата обновления не может быть меньше даты создания.",
                    nameof(updatedAt));
            }

            return new EntityLifeTime(createdAt, updatedAt, isActive);
        }

        /// <summary>
        /// Создает новый жизненный цикл для новой сущности (CreatedAt = UpdatedAt = Now).
        /// </summary>
        /// <param name="isActive">Начальное состояние активности.</param>
        /// <returns>Новый экземпляр <see cref="EntityLifeTime"/>.</returns>
        public static EntityLifeTime CreateNew(bool isActive = true)
        {
            var now = DateTime.UtcNow;
            return new EntityLifeTime(now, now, isActive);
        }

        /// <summary>
        /// Обновляет дату изменения и состояние активности.
        /// </summary>
        /// <param name="isActive">Новое состояние активности (null = не менять).</param>
        /// <returns>Новый экземпляр с обновленными данными.</returns>
        public EntityLifeTime Update(bool? isActive = null)
        {
            return new EntityLifeTime(
                CreatedAt,
                DateTime.UtcNow,
                isActive ?? IsActive);
        }

        /// <summary>
        /// Архивирует сущность (IsActive = false).
        /// </summary>
        /// <returns>Новый экземпляр с архивированным состоянием.</returns>
        public EntityLifeTime Archive()
        {
            return new EntityLifeTime(CreatedAt, DateTime.UtcNow, false);
        }

        /// <summary>
        /// Разархивирует сущность (IsActive = true).
        /// </summary>
        /// <returns>Новый экземпляр с активным состоянием.</returns>
        public EntityLifeTime Unarchive()
        {
            return new EntityLifeTime(CreatedAt, DateTime.UtcNow, true);
        }

        public override string ToString() =>
            $"[Created: {CreatedAt:yyyy-MM-dd HH:mm:ss}, Updated: {UpdatedAt:yyyy-MM-dd HH:mm:ss}, Active: {IsActive}]";

        private static void ValidateDateTime(DateTime value, string paramName, string description)
        {
            if (value == DateTime.MinValue || value == DateTime.MaxValue || value == default)
            {
                throw new ArgumentException(
                    $"Некорректное значение даты {description}.",
                    paramName);
            }
        }
    }
}