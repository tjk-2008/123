using System;

namespace DirectoryService.Domain.DepartmentsContext.ValueObjects
{
    /// <summary>
    /// Название подразделения.
    /// </summary>
    public sealed record DepartmentName
    {
        /// <summary>
        /// Максимальная длина названия подразделения.
        /// </summary>
        public const int MaxLength = 128;

        /// <summary>
        /// Минимальная длина названия подразделения.
        /// </summary>
        public const int MinLength = 2;

        /// <summary>
        /// Получает значение названия подразделения.
        /// </summary>
        public string Value { get; }

        private DepartmentName(string value) => Value = value;

        /// <summary>
        /// Создает новое название подразделения.
        /// </summary>
        /// <param name="value">Строковое значение названия.</param>
        /// <returns>Экземпляр <see cref="DepartmentName"/>.</returns>
        /// <exception cref="ArgumentException">
        /// Выбрасывается при невалидном значении названия.
        /// </exception>
        public static DepartmentName Create(string value)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);

            // Trim ДО всех проверок длины!
            var trimmed = value.Trim();

            if (trimmed.Length is < MinLength or > MaxLength)
            {
                throw new ArgumentException(
                    $"Название должно быть от {MinLength} до {MaxLength} символов после удаления пробелов.",
                    nameof(value));
            }

            return new DepartmentName(trimmed);
        }

        /// <summary>
        /// Сравнивает названия без учета регистра.
        /// </summary>
        /// <param name="other">Другое название.</param>
        /// <returns>true, если равны без учета регистра.</returns>
        public bool EqualsCaseInsensitive(DepartmentName? other) =>
            other is not null &&
            string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);

        public static implicit operator string(DepartmentName name) => name.Value;

        public override string ToString() => Value;
    }
}