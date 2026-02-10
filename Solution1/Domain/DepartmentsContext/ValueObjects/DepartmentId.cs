using System;

namespace DirectoryService.Domain.DepartmentsContext.ValueObjects
{
    /// <summary>
    /// Уникальный идентификатор подразделения.
    /// </summary>
    public sealed record DepartmentId
    {
        /// <summary>
        /// Получает значение идентификатора.
        /// </summary>
        public Guid Value { get; }

        private DepartmentId(Guid value) => Value = value;

        /// <summary>
        /// Создает новый уникальный идентификатор подразделения.
        /// </summary>
        /// <returns>Новый экземпляр <see cref="DepartmentId"/>.</returns>
        public static DepartmentId Create() => new(Guid.NewGuid());

        /// <summary>
        /// Создает идентификатор подразделения из существующего GUID.
        /// </summary>
        /// <param name="value">Значение GUID.</param>
        /// <returns>Экземпляр <see cref="DepartmentId"/>.</returns>
        /// <exception cref="ArgumentException">
        /// Выбрасывается, когда значение GUID пустое.
        /// </exception>
        public static DepartmentId Create(Guid value)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException(
                    "Идентификатор не может быть пустым.",
                    nameof(value));
            }

            return new DepartmentId(value);
        }

        public static implicit operator Guid(DepartmentId id) => id.Value;

        public static implicit operator string(DepartmentId id) => id.Value.ToString("D");

        public override string ToString() => Value.ToString("D");
    }
}