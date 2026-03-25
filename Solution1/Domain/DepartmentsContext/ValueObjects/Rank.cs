namespace DirectoryService.Domain.DepartmentsContext.ValueObjects
{
    public sealed record Rank
    {
        public const int MinRank = 1;
        public const int MaxRank = 100;

        public int Value { get; }

        private Rank(int value)
        {
            Value = value;
        }

        public static Rank Create(int value)
        {
            if (value < MinRank || value > MaxRank)
            {
                throw new ArgumentException($"Ранг должен быть от {MinRank} до {MaxRank}");
            }

            return new Rank(value);
        }

        public Rank Increase()
        {
            if (Value == MinRank)
            {
                throw new InvalidOperationException("Нельзя повысить ранг выше максимального");
            }

            return new Rank(Value - 1);
        }

        public Rank Decrease()
        {
            if (Value == MaxRank)
            {
                throw new InvalidOperationException("Нельзя понизить ранг ниже минимального");
            }

            return new Rank(Value + 1);
        }
    }
}