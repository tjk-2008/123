using DirectoryService.Domain.LocationsContext;
using DirectoryService.Domain.LocationsContext.ValueObjects;
using DirectoryService.Domain.Shared;

namespace Tests
{
    public class UnitLocationTests
    {
        #region LocationName Tests
        [Theory]
        [InlineData("Москва")]
        [InlineData("Нью-Йорк")]
        public void Should_Create_When_ValidName(string validName)
        {
            LocationName name = LocationName.Create(validName);
            Assert.Equal(validName, name.Value);
        }

        [Theory]
        [InlineData("")]
        public void Should_Throw_When_NameIsEmpty(string invalidName)
        {
            Assert.Throws<ArgumentException>(() => LocationName.Create(invalidName));
        }

        [Fact]
        public void Should_Throw_When_NameIsNull()
        {
            Assert.Throws<ArgumentException>(() => LocationName.Create(null!));
        }
        #endregion

        #region LocationId Tests
        [Fact]
        public void Should_Create_When_NewGuid()
        {
            LocationId id1 = LocationId.Create();
            LocationId id2 = LocationId.Create();
            Assert.NotEqual(id1, id2);
        }

        [Fact]
        public void Should_Throw_When_GuidIsEmpty()
        {
            Assert.Throws<ArgumentException>(() => LocationId.Create(Guid.Empty));
        }
        #endregion

        #region IanaTimeZone Tests
        [Theory]
        [InlineData("Europe/Moscow")]
        [InlineData("America/New_York")]
        public void Should_Create_When_ValidTimeZone(string valid)
        {
            IanaTimeZone tz = IanaTimeZone.Create(valid);
            Assert.Equal(valid, tz.Value);
        }

        [Theory]
        [InlineData("")]
        [InlineData("Invalid")]
        public void Should_Throw_When_InvalidTimeZone(string invalid)
        {
            Assert.Throws<ArgumentException>(() => IanaTimeZone.Create(invalid));
        }
        #endregion

        #region LocationAddress Tests
        [Theory]
        [InlineData("ул. Ленина, Москва")]
        public void Should_Create_When_ValidAddress(string valid)
        {
            LocationAddress address = LocationAddress.Create(valid);
            Assert.Equal(valid, address.Value);
        }

        [Theory]
        [InlineData("")]
        [InlineData(",")]
        public void Should_Throw_When_InvalidAddress(string invalid)
        {
            Assert.Throws<ArgumentException>(() => LocationAddress.Create(invalid));
        }
        #endregion

        #region Location Entity Tests
        private Location CreateTestLocation()
        {
            return new Location(
                LocationId.Create(),
                LocationAddress.Create("ул. Ленина, 1"),
                LocationName.Create("Москва"),
                IanaTimeZone.Create("Europe/Moscow"),
                EntityLifeTime.Create()
            );
        }

        [Fact]
        public void Should_UpdateName_When_LocationIsActive()
        {
            Location location = CreateTestLocation();
            LocationName newName = LocationName.Create("Питер");

            location.ChangeName(newName);

            Assert.Equal(newName, location.Name);
        }

        [Fact]
        public void Should_Throw_When_ChangeNameAndLocationIsNotActive()
        {
            EntityLifeTime inactiveLifeTime = EntityLifeTime.Create(
                DateTime.UtcNow,
                DateTime.UtcNow,
                false
            );

            Location location = new Location(
                LocationId.Create(),
                LocationAddress.Create("ул. Ленина, 1"),
                LocationName.Create("Москва"),
                IanaTimeZone.Create("Europe/Moscow"),
                inactiveLifeTime
            );

            Assert.Throws<InvalidOperationException>(() =>
                location.ChangeName(LocationName.Create("Питер")));
        }
        #endregion
    }
}