namespace DirectoryService.Domain.LocationsContext.ValueObjects
{
    public sealed record IanaTimeZone
    {
        public string Value { get; }

        // Конструктор без параметров для EF Core
        private IanaTimeZone()
        {
            Value = string.Empty;
        }

        private IanaTimeZone(string value)
        {
            Value = value;
        }

        public static IanaTimeZone Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("IANA временная зона не может быть пустой.", nameof(value));
            }

            if (!value.Contains('/', StringComparison.Ordinal))
            {
                throw new ArgumentException("Некорректный формат IANA временной зоны.", nameof(value));
            }

            string[] parts = value.Split('/');
            if (parts.Length != 2)
            {
                throw new ArgumentException("Некорректный формат IANA временной зоны.", nameof(value));
            }

            if (parts.Any(string.IsNullOrWhiteSpace))
            {
                throw new ArgumentException("Некорректный формат IANA временной зоны.", nameof(value));
            }

            return new IanaTimeZone(value);
        }

        public static IanaTimeZone ChangeIana(string name)
        {
            return Create(name);
        }
    }
}