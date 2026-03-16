using Xunit;
using System;

namespace Tests
{
    public sealed record PositionRank
    {
        public const int MinRank = 1;
        public const int MaxRank = 100;

        public int Value { get; }

        private PositionRank(int value)
        {
            Value = value;
        }

        public static PositionRank Create(int value)
        {
            if (value < MinRank || value > MaxRank)
            {
                throw new ArgumentException($"Ранг должен быть от {MinRank} до {MaxRank}");
            }

            return new PositionRank(value);
        }

        public PositionRank Increase()
        {
            if (Value == MinRank)
            {
                throw new InvalidOperationException("Нельзя повысить ранг выше максимального");
            }

            return new PositionRank(Value - 1);
        }

        public PositionRank Decrease()
        {
            if (Value == MaxRank)
            {
                throw new InvalidOperationException("Нельзя понизить ранг ниже минимального");
            }

            return new PositionRank(Value + 1);
        }
    }
}

namespace Tests
{
    public class RankTest
    {
        [Theory]
        [InlineData(1)]
        [InlineData(50)]
        [InlineData(100)]
        public void Should_Create_When_ValidRank(int value)
        {
            PositionRank rank = PositionRank.Create(value);
            Assert.Equal(value, rank.Value);
        }

        [Fact]
        public void Should_Throw_When_RankIsTooSmall()
        {
            Assert.Throws<ArgumentException>(() => PositionRank.Create(0));
        }

        [Fact]
        public void Should_Throw_When_RankIsTooLarge()
        {
            Assert.Throws<ArgumentException>(() => PositionRank.Create(101));
        }

        [Fact]
        public void Should_Increase_When_RankGoesUp()
        {
            PositionRank rank = PositionRank.Create(10);
            PositionRank increased = rank.Increase();
            Assert.Equal(9, increased.Value);
        }

        [Fact]
        public void Should_Throw_When_IncreaseMaxRank()
        {
            PositionRank rank = PositionRank.Create(1);
            Assert.Throws<InvalidOperationException>(rank.Increase);
        }

        [Fact]
        public void Should_Decrease_When_RankGoesDown()
        {
            PositionRank rank = PositionRank.Create(10);
            PositionRank decreased = rank.Decrease();
            Assert.Equal(11, decreased.Value);
        }

        [Fact]
        public void Should_Throw_When_DecreaseMinRank()
        {
            PositionRank rank = PositionRank.Create(100);
            Assert.Throws<InvalidOperationException>(rank.Decrease);
        }
    }
}