namespace DirectoryService.Domain.DepartmentsContext.ValueObjects
{
    /// <summary>
    /// Глубина подразделения в иерархии.
    /// </summary>
    public sealed record DepartmentDepth
    {
        /// <summary>
        /// Максимальная глубина иерархии.
        /// </summary>
        public const short MaxDepth = 10;

        /// <summary>
        /// Получает значение глубины.
        /// </summary>
        public short Value { get; }

        private DepartmentDepth(short value) => Value = value;

        /// <summary>
        /// Создает глубину подразделения.
        /// </summary>
        /// <param name="value">Значение глубины.</param>
        /// <returns>Экземпляр <see cref="DepartmentDepth"/>.</returns>
        public static DepartmentDepth Create(short value)
        {
            if (value < 0)
                throw new ArgumentException("Глубина не может быть отрицательной.", nameof(value));

            if (value > MaxDepth)
                throw new ArgumentException($"Глубина не может превышать {MaxDepth}.", nameof(value));

            return new DepartmentDepth(value);
        }

        /// <summary>
        /// Вычисляет глубину из пути.
        /// </summary>
        /// <param name="path">Путь подразделения.</param>
        /// <returns>Глубина подразделения.</returns>
        public static DepartmentDepth CalculateFromPath(DepartmentPath path)
        {
            ArgumentNullException.ThrowIfNull(path);

            var depth = path.CalculateDepth();
            return Create(depth);
        }

        /// <summary>
        /// Увеличивает глубину на 1.
        /// </summary>
        /// <returns>Новая глубина.</returns>
        /// <exception cref="InvalidOperationException">Выбрасывается при достижении максимума.</exception>
        public DepartmentDepth Increment()
        {
            if (Value >= MaxDepth)
                throw new InvalidOperationException($"Достигнута максимальная глубина {MaxDepth}.");

            return Create((short)(Value + 1));
        }

        public static implicit operator short(DepartmentDepth depth) => depth.Value;

        public override string ToString() => Value.ToString();
    }
}