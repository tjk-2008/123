namespace DirectoryService.Domain.LocationsContext.ValueObjects
{
    public sealed record LocationAddress
    {
        private readonly List<string> _addressParts = new();

        public string Value { get; }

        public IReadOnlyList<string> AddressParts => _addressParts.AsReadOnly();

        private LocationAddress(IEnumerable<string> parts)
        {
            _addressParts = parts.ToList();
            Value = string.Join(", ", parts);
        }

        public static LocationAddress Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Адрес локации не может быть пустым.", nameof(value));
            }

            List<string> parts = value
                .Split(',')
                .Select(part => part.Trim())
                .Where(part => !string.IsNullOrWhiteSpace(part))
                .ToList();

            if (parts.Count == 0)
            {
                throw new ArgumentException("Адрес локации должен содержать хотя бы одну часть.", nameof(value));
            }

            return new LocationAddress(parts);
        }

        public static LocationAddress CreateWithoutMultipleEnumerations(IEnumerable<string> parts)
        {
            ArgumentNullException.ThrowIfNull(parts);

            // Создаем список, но не заполняем его сразу
            List<string> resultParts = new List<string>();

            foreach (string part in parts)
            {
                if (part == null)
                {
                    throw new ArgumentException("Часть адреса не может быть null.", nameof(parts));
                }

                string trimmed = part.Trim();
                if (!string.IsNullOrWhiteSpace(trimmed))
                {
                    resultParts.Add(trimmed);
                }
            }

            if (resultParts.Count == 0)
            {
                throw new ArgumentException("Адрес локации должен содержать хотя бы одну часть.", nameof(parts));
            }

            return new LocationAddress(resultParts);
        }
    }
}
