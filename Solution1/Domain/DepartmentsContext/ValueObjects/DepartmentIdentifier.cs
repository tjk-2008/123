using System;
using System.Text.RegularExpressions;

namespace DirectoryService.Domain.DepartmentsContext.ValueObjects
{
    /// <summary>
    /// Строковый идентификатор подразделения (slug) для использования в путях и URL.
    /// </summary>
    public sealed record DepartmentIdentifier
    {
        // Улучшенный паттерн: не начинается/не заканчивается на '-', нет '--'
        private static readonly Regex PatternRegex = new(
            @"^[a-z0-9]+(-[a-z0-9]+)*$",
            RegexOptions.Compiled);

        /// <summary>
        /// Максимальная длина идентификатора.
        /// </summary>
        public const int MaxLength = 50;

        /// <summary>
        /// Минимальная длина идентификатора.
        /// </summary>
        public const int MinLength = 2;

        /// <summary>
        /// Получает значение идентификатора.
        /// </summary>
        public string Value { get; }

        private DepartmentIdentifier(string value) => Value = value;

        /// <summary>
        /// Создает новый идентификатор подразделения.
        /// </summary>
        /// <param name="value">Строковое значение идентификатора.</param>
        /// <returns>Экземпляр <see cref="DepartmentIdentifier"/>.</returns>
        /// <exception cref="ArgumentException">
        /// Выбрасывается при невалидном значении идентификатора.
        /// </exception>
        public static DepartmentIdentifier Create(string value)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);

            if (value.Length is < MinLength or > MaxLength)
            {
                throw new ArgumentException(
                    $"Идентификатор должен быть от {MinLength} до {MaxLength} символов.",
                    nameof(value));
            }

            if (!PatternRegex.IsMatch(value))
            {
                throw new ArgumentException(
                    "Идентификатор должен содержать строчные буквы и цифры, " +
                    "разделённые одиночными дефисами. Не может начинаться/заканчиваться дефисом.",
                    nameof(value));
            }

            return new DepartmentIdentifier(value);
        }

        /// <summary>
        /// Создает идентификатор на основе названия подразделения.
        /// </summary>
        /// <param name="departmentName">Название подразделения.</param>
        /// <returns>Экземпляр <see cref="DepartmentIdentifier"/>.</returns>
        public static DepartmentIdentifier CreateFromName(DepartmentName departmentName)
        {
            ArgumentNullException.ThrowIfNull(departmentName);

            var slug = ToSlug(departmentName.Value);

            // Если не удалось создать валидный slug — fallback на безопасный
            if (!TryCreateSafe(slug, out var identifier))
            {
                slug = GenerateFallbackSlug();
                identifier = new DepartmentIdentifier(slug); // напрямую, без Create()
            }

            return identifier;
        }

        private static string ToSlug(string name)
        {
            var slug = name.ToLowerInvariant();

            // Удаляем всё кроме букв, цифр, пробелов, дефисов
            slug = Regex.Replace(slug, @"[^a-z0-9\s-]", string.Empty);

            // Нормализуем пробелы и дефисы в одиночные дефисы
            slug = Regex.Replace(slug, @"[\s-]+", "-").Trim('-');

            // Обрезаем с умом — не посреди слова
            if (slug.Length > MaxLength)
            {
                slug = slug[..MaxLength].TrimEnd('-');
                var lastDash = slug.LastIndexOf('-');
                if (lastDash >= MinLength)
                {
                    slug = slug[..lastDash];
                }
            }

            return slug;
        }

        private static bool TryCreateSafe(string slug, out DepartmentIdentifier? identifier)
        {
            identifier = null;

            if (slug.Length < MinLength || slug.Length > MaxLength)
                return false;

            if (!PatternRegex.IsMatch(slug))
                return false;

            identifier = new DepartmentIdentifier(slug);
            return true;
        }

        private static string GenerateFallbackSlug()
        {
            // Короткий, безопасный, предсказуемый формат
            return $"dept-{Guid.NewGuid().ToString("n")[..8]}";
        }

        public static implicit operator string(DepartmentIdentifier? identifier) =>
            identifier?.Value ?? string.Empty;

        public override string ToString() => Value;
    }
}