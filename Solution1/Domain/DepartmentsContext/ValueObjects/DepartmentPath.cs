namespace DirectoryService.Domain.DepartmentsContext.ValueObjects
{
    /// <summary>
    /// Путь подразделения в иерархической структуре.
    /// </summary>
    public sealed record DepartmentPath
    {
        /// <summary>
        /// Максимальная длина пути подразделения.
        /// </summary>
        public const int MaxLength = 500;

        /// <summary>
        /// Разделитель частей пути.
        /// </summary>
        private const char Separator = '.';

        /// <summary>
        /// Получает значение пути подразделения.
        /// </summary>
        public string Value { get; }

        private DepartmentPath(string value) => Value = value;

        /// <summary>
        /// Создает новый путь подразделения.
        /// </summary>
        /// <param name="value">Строковое значение пути.</param>
        /// <returns>Экземпляр <see cref="DepartmentPath"/>.</returns>
        /// <exception cref="ArgumentException">
        /// Выбрасывается при невалидном значении пути.
        /// </exception>
        public static DepartmentPath Create(string value)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);

            if (value.Length > MaxLength)
            {
                throw new ArgumentException(
                    $"Путь не может превышать {MaxLength} символов.",
                    nameof(value));
            }

            var parts = value.Split(Separator, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0)
            {
                throw new ArgumentException("Путь не может быть пустым.", nameof(value));
            }

            // Проверяем каждую часть на валидность как DepartmentIdentifier
            foreach (var part in parts)
            {
                if (part.Length > DepartmentIdentifier.MaxLength)
                {
                    throw new ArgumentException(
                        $"Часть пути '{part}' превышает максимальную длину.",
                        nameof(value));
                }
            }

            // Проверяем на дубликаты (циклическую ссылку)
            var distinctParts = parts.Distinct(StringComparer.Ordinal).ToList();
            if (distinctParts.Count != parts.Length)
            {
                throw new ArgumentException(
                    "Путь содержит повторяющиеся идентификаторы.",
                    nameof(value));
            }

            return new DepartmentPath(value);
        }

        /// <summary>
        /// Создает путь для корневого подразделения.
        /// </summary>
        /// <param name="identifier">Идентификатор корневого подразделения.</param>
        /// <returns>Экземпляр <see cref="DepartmentPath"/> для корня.</returns>
        public static DepartmentPath CreateRoot(DepartmentIdentifier identifier)
        {
            ArgumentNullException.ThrowIfNull(identifier);
            return Create(identifier.Value);
        }

        /// <summary>
        /// Создает путь для дочернего подразделения.
        /// </summary>
        /// <param name="parentPath">Путь родительского подразделения.</param>
        /// <param name="childIdentifier">Идентификатор дочернего подразделения.</param>
        /// <returns>Экземпляр <see cref="DepartmentPath"/> для потомка.</returns>
        public static DepartmentPath CreateChild(DepartmentPath parentPath, DepartmentIdentifier childIdentifier)
        {
            ArgumentNullException.ThrowIfNull(parentPath);
            ArgumentNullException.ThrowIfNull(childIdentifier);

            // Проверяем, что не создаем цикл
            if (parentPath.ContainsIdentifier(childIdentifier.Value))
            {
                throw new InvalidOperationException(
                    "Нельзя создать циклическую зависимость в пути.");
            }

            var newPath = $"{parentPath.Value}{Separator}{childIdentifier.Value}";
            return Create(newPath);
        }

        /// <summary>
        /// Глубина: 0 для корня, 1 для первого потомка и т.д.
        /// </summary>
        public short CalculateDepth() => (short)Value.Count(c => c == Separator);

        /// <summary>
        /// Получает путь родительского подразделения.
        /// </summary>
        /// <returns>Путь родительского подразделения или null для корня.</returns>
        public DepartmentPath? GetParentPath()
        {
            var lastIndex = Value.LastIndexOf(Separator);
            return lastIndex > 0
                ? Create(Value[..lastIndex])
                : null;
        }

        /// <summary>
        /// Получает идентификатор подразделения из пути (последняя часть).
        /// </summary>
        /// <returns>Идентификатор подразделения.</returns>
        public DepartmentIdentifier GetIdentifier()
        {
            var lastIndex = Value.LastIndexOf(Separator);
            var id = lastIndex >= 0
                ? Value[(lastIndex + 1)..]
                : Value;
            return DepartmentIdentifier.Create(id);
        }

        /// <summary>
        /// Проверяет, является ли текущий путь потомком указанного пути.
        /// </summary>
        /// <param name="parentPath">Потенциальный путь родителя.</param>
        /// <returns>true, если текущий путь является потомком; иначе false.</returns>
        public bool IsDescendantOf(DepartmentPath? parentPath)
        {
            if (parentPath is null) return false;

            return Value.StartsWith(
                $"{parentPath.Value}{Separator}",
                StringComparison.Ordinal);
        }

        /// <summary>
        /// Проверяет, является ли текущий путь предком указанного пути.
        /// </summary>
        /// <param name="childPath">Потенциальный путь потомка.</param>
        /// <returns>true, если текущий путь является предком; иначе false.</returns>
        public bool IsAncestorOf(DepartmentPath? childPath) =>
            childPath?.IsDescendantOf(this) ?? false;

        /// <summary>
        /// Проверяет, содержит ли путь указанный идентификатор.
        /// </summary>
        /// <param name="identifier">Идентификатор для проверки.</param>
        /// <returns>true, если содержит; иначе false.</returns>
        public bool ContainsIdentifier(string identifier) =>
            GetPathParts().Contains(identifier, StringComparer.Ordinal);

        /// <summary>
        /// Разбивает путь на части (идентификаторы).
        /// </summary>
        /// <returns>Массив идентификаторов в пути.</returns>
        public string[] GetPathParts() =>
            Value.Split(Separator, StringSplitOptions.RemoveEmptyEntries);

        public static implicit operator string(DepartmentPath? path) => path?.Value ?? string.Empty;

        public override string ToString() => Value;
    }
}