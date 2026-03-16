using DirectoryService.Domain.PositionsContext;
using DirectoryService.Domain.PositionsContext.ValueObjects;
using DirectoryService.Domain.Shared;

namespace Tests
{
    public class UnitPositionsTests
    {
        #region PositionDescription Tests
        [Theory]
        [InlineData("Описание")]
        public void Should_Create_When_ValidDescription(string valid)
        {
            PositionDescription desc = PositionDescription.Create(valid);
            Assert.Equal(valid, desc.Value);
        }

        [Fact]
        public void Should_ReturnEmpty_When_DescriptionIsNull()
        {
            PositionDescription desc = PositionDescription.Create(null!);
            Assert.Equal(string.Empty, desc.Value);
        }

        [Fact]
        public void Should_Throw_When_DescriptionExceedsMaxLength()
        {
            Assert.Throws<ArgumentException>(() =>
                PositionDescription.Create(new string('A', 501)));
        }

        [Fact]
        public void Should_ReturnEmpty_When_CreateEmpty()
        {
            PositionDescription empty = PositionDescription.Empty();
            Assert.Equal(string.Empty, empty.Value);
        }
        #endregion

        #region PositionId Tests
        [Fact]
        public void Should_Create_When_NewGuid()
        {
            Assert.NotEqual(PositionId.Create(), PositionId.Create());
        }

        [Fact]
        public void Should_Create_When_ValidGuid()
        {
            Guid guid = Guid.NewGuid();
            PositionId id = PositionId.Create(guid);
            Assert.Equal(guid, id.Value);
        }

        [Fact]
        public void Should_Throw_When_GuidIsEmpty()
        {
            Assert.Throws<ArgumentException>(() => PositionId.Create(Guid.Empty));
        }
        #endregion

        #region PositionName Tests
        [Theory]
        [InlineData("Разработчик")]
        public void Should_Create_When_ValidName(string valid)
        {
            PositionName name = PositionName.Create(valid);
            Assert.Equal(valid, name.Value);
        }

        [Theory]
        [InlineData("")]
        public void Should_Throw_When_NameIsEmpty(string invalid)
        {
            Assert.Throws<ArgumentException>(() => PositionName.Create(invalid));
        }

        [Fact]
        public void Should_Throw_When_NameIsNull()
        {
            Assert.Throws<ArgumentException>(() => PositionName.Create(null!));
        }
        #endregion

        #region Position Entity Tests
        private Position CreateTestPosition()
        {
            return new Position(
                PositionId.Create(),
                PositionName.Create("Разработчик"),
                PositionDescription.Create("Описание"),
                true,
                EntityLifeTime.Create()
            );
        }

        [Fact]
        public void Should_UpdateName_When_PositionIsActive()
        {
            Position pos = CreateTestPosition();
            PositionName newName = PositionName.Create("Старший");

            pos.ChangePositionName(newName);

            Assert.Equal(newName, pos.Name);
        }

        [Fact]
        public void Should_Throw_When_ChangeNameWithDelegateAndNameNotUnique()
        {
            Position pos = CreateTestPosition();

            Assert.Throws<InvalidOperationException>(() =>
                pos.ChangePositionName(PositionName.Create("Новое"), (n) => false));
        }
        #endregion
    }
}