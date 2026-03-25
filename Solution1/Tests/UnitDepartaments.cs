using DirectoryService.Domain.DepartmentsContext;
using DirectoryService.Domain.DepartmentsContext.ValueObjects;
using DirectoryService.Domain.LocationsContext;
using DirectoryService.Domain.LocationsContext.ValueObjects;
using DirectoryService.Domain.PositionsContext;
using DirectoryService.Domain.PositionsContext.ValueObjects;
using DirectoryService.Domain.Shared;

namespace Tests
{
    public class UnitDepartmentsTests
    {
        #region DepartmentDepth Tests
        [Theory]
        [InlineData(0)]
        [InlineData(5)]
        public void Should_CreateDepth_When_Valid(short valid)
        {
            DepartmentDepth depth = DepartmentDepth.Create(valid);
            Assert.Equal(valid, depth.Value);
        }

        [Fact]
        public void Should_Throw_When_DepthNegative()
        {
            Assert.Throws<ArgumentException>(() => DepartmentDepth.Create(-1));
        }

        [Fact]
        public void Should_IncreaseDepth()
        {
            DepartmentDepth depth = DepartmentDepth.Create(5);
            DepartmentDepth incremented = depth.Increment();
            Assert.Equal(6, incremented.Value);
        }
        #endregion

        #region DepartmentId Tests
        [Fact]
        public void Should_CreateId_When_NewGuid()
        {
            DepartmentId id1 = DepartmentId.Create();
            DepartmentId id2 = DepartmentId.Create();
            Assert.NotEqual(id1, id2);
        }

        [Fact]
        public void Should_Throw_When_IdEmpty()
        {
            Assert.Throws<ArgumentException>(() => DepartmentId.Create(Guid.Empty));
        }

        [Fact]
        public void Should_ReturnNull_When_CreateNullableWithNull()
        {
            DepartmentId? result = DepartmentId.CreateNullable(null);
            Assert.Null(result);
        }
        #endregion

        #region DepartmentIdentifier Tests
        [Theory]
        [InlineData("it")]
        [InlineData("hr")]
        public void Should_CreateIdentifier_When_Valid(string valid)
        {
            DepartmentIdentifier identifier = DepartmentIdentifier.Create(valid);
            Assert.Equal(valid, identifier.Value);
        }

        [Theory]
        [InlineData("")]
        [InlineData("IT")]
        public void Should_Throw_When_IdentifierInvalid(string invalid)
        {
            Assert.Throws<ArgumentException>(() => DepartmentIdentifier.Create(invalid));
        }
        #endregion

        #region DepartmentName Tests
        [Theory]
        [InlineData("IT")]
        [InlineData("Sales")]
        public void Should_CreateName_When_Valid(string valid)
        {
            DepartmentName name = DepartmentName.Create(valid);
            Assert.Equal(valid, name.Value);
        }

        [Theory]
        [InlineData("")]
        [InlineData("A")]
        public void Should_Throw_When_NameInvalid(string invalid)
        {
            Assert.Throws<ArgumentException>(() => DepartmentName.Create(invalid));
        }
        #endregion

        #region DepartmentPath Tests
        [Theory]
        [InlineData("it")]
        [InlineData("it.dev")]
        public void Should_CreatePath_When_Valid(string valid)
        {
            DepartmentPath path = DepartmentPath.Create(valid);
            Assert.Equal(valid, path.Value);
        }

        [Theory]
        [InlineData("")]
        [InlineData("it..dev")]
        public void Should_Throw_When_PathInvalid(string invalid)
        {
            Assert.Throws<ArgumentException>(() => DepartmentPath.Create(invalid));
        }

        [Fact]
        public void Should_CreateChildPath()
        {
            DepartmentPath parent = DepartmentPath.Create("it");
            DepartmentPath child = DepartmentPath.CreateForChild(parent, "dev");
            Assert.Equal("it.dev", child.Value);
        }

        [Fact]
        public void Should_CalculateDepth()
        {
            DepartmentPath path = DepartmentPath.Create("it.dev.backend");
            short depth = path.CalculateDepth();
            Assert.Equal(3, depth);
        }
        #endregion

        #region Rank Tests
        [Theory]
        [InlineData(1)]
        [InlineData(50)]
        [InlineData(100)]
        public void Should_CreateRank_When_Valid(int value)
        {
            Rank rank = Rank.Create(value);
            Assert.Equal(value, rank.Value);
        }

        [Fact]
        public void Should_Throw_When_RankTooSmall()
        {
            Assert.Throws<ArgumentException>(() => Rank.Create(0));
        }

        [Fact]
        public void Should_Throw_When_RankTooLarge()
        {
            Assert.Throws<ArgumentException>(() => Rank.Create(101));
        }

        [Fact]
        public void Should_IncreaseRank()
        {
            Rank rank = Rank.Create(10);
            Rank increased = rank.Increase();
            Assert.Equal(9, increased.Value);
        }

        [Fact]
        public void Should_Throw_When_IncreaseMinRank()
        {
            Rank rank = Rank.Create(1);
            Assert.Throws<InvalidOperationException>(rank.Increase);
        }

        [Fact]
        public void Should_DecreaseRank()
        {
            Rank rank = Rank.Create(10);
            Rank decreased = rank.Decrease();
            Assert.Equal(11, decreased.Value);
        }

        [Fact]
        public void Should_Throw_When_DecreaseMaxRank()
        {
            Rank rank = Rank.Create(100);
            Assert.Throws<InvalidOperationException>(rank.Decrease);
        }
        #endregion

        #region Department Entity Tests
        private Department CreateTestDepartment()
        {
            DepartmentName name = DepartmentName.Create("IT");
            DepartmentIdentifier identifier = DepartmentIdentifier.Create("it");
            Department department = Department.CreateRoot(name, identifier);
            return department;
        }

        private Position CreateTestPosition(string name)
        {
            PositionId id = PositionId.Create();
            PositionName positionName = PositionName.Create(name);
            PositionDescription description = PositionDescription.Create("");
            EntityLifeTime lifeTime = EntityLifeTime.Create();
            Position position = new Position(id, positionName, description, true, lifeTime);
            return position;
        }

        private Location CreateTestLocation()
        {
            LocationId id = LocationId.Create();
            LocationAddress address = LocationAddress.Create("ул. Ленина, 1");
            LocationName name = LocationName.Create("Москва");
            IanaTimeZone timeZone = IanaTimeZone.Create("Europe/Moscow");
            EntityLifeTime lifeTime = EntityLifeTime.Create();
            Location location = new Location(id, address, name, timeZone, lifeTime);
            return location;
        }

        [Fact]
        public void Should_CreateRootDepartment()
        {
            Department department = CreateTestDepartment();
            Assert.True(department.IsRoot());
            Assert.Equal("it", department.Path.Value);
            Assert.Equal(1, department.Depth.Value);
        }

        [Fact]
        public void Should_CreateChildDepartment()
        {
            Department parent = CreateTestDepartment();
            DepartmentName childName = DepartmentName.Create("Dev");
            DepartmentIdentifier childIdentifier = DepartmentIdentifier.Create("dev");
            Department child = Department.CreateChild(childName, childIdentifier, parent);
            Assert.Equal(parent.Id, child.ParentId);
            Assert.Equal("it.dev", child.Path.Value);
        }

        [Fact]
        public void Should_AddPosition_WithRank()
        {
            Department department = CreateTestDepartment();
            Position position = CreateTestPosition("Developer");
            Rank rank = Rank.Create(5);

            department.AddPosition(position, rank);

            Assert.Single(department.Positions);
            Assert.Equal(rank, department.Positions[0].PositionRank);
        }

        [Fact]
        public void Should_AddPosition_WithRankValue()
        {
            Department department = CreateTestDepartment();
            Position position = CreateTestPosition("Developer");

            department.AddPosition(position, 5);

            Assert.Single(department.Positions);
            Assert.Equal(5, department.Positions[0].PositionRank.Value);
        }

        [Fact]
        public void Should_Throw_When_AddingDuplicatePosition()
        {
            Department department = CreateTestDepartment();
            Position position = CreateTestPosition("Developer");
            Rank rank = Rank.Create(5);

            department.AddPosition(position, rank);
            Assert.Throws<ArgumentException>(() => department.AddPosition(position, rank));
        }

        [Fact]
        public void Should_AddPosition_WithSameRank_Allowed()
        {
            Department department = CreateTestDepartment();
            Position position1 = CreateTestPosition("Developer");
            Position position2 = CreateTestPosition("Manager");
            Rank rank = Rank.Create(5);

            department.AddPosition(position1, rank);
            department.AddPosition(position2, rank);

            Assert.Equal(2, department.Positions.Count);
        }

        [Fact]
        public void Should_ChangePositionRank()
        {
            Department department = CreateTestDepartment();
            Position position = CreateTestPosition("Developer");
            Rank oldRank = Rank.Create(5);
            Rank newRank = Rank.Create(10);

            department.AddPosition(position, oldRank);
            department.ChangePositionRank(position.Id, newRank);

            Assert.Equal(newRank, department.Positions[0].PositionRank);
        }

        [Fact]
        public void Should_ChangePositionRank_ByValue()
        {
            Department department = CreateTestDepartment();
            Position position = CreateTestPosition("Developer");

            department.AddPosition(position, 5);
            department.ChangePositionRank(position.Id, 10);

            Assert.Equal(10, department.Positions[0].PositionRank.Value);
        }

        [Fact]
        public void Should_RemovePosition()
        {
            Department department = CreateTestDepartment();
            Position position = CreateTestPosition("Developer");
            Rank rank = Rank.Create(5);

            department.AddPosition(position, rank);
            department.RemovePosition(position.Id);

            Assert.Empty(department.Positions);
        }

        [Fact]
        public void Should_AddLocation()
        {
            Department department = CreateTestDepartment();
            Location location = CreateTestLocation();

            department.AddLocation(location);

            Assert.Single(department.Locations);
            Assert.Equal(location.Id, department.Locations[0].LocationId);
        }

        [Fact]
        public void Should_Throw_When_AddingDuplicateLocation()
        {
            Department department = CreateTestDepartment();
            Location location = CreateTestLocation();

            department.AddLocation(location);
            Assert.Throws<ArgumentException>(() => department.AddLocation(location));
        }

        [Fact]
        public void Should_Throw_When_AddLocation_ToArchivedDepartment()
        {
            Department department = CreateTestDepartment();
            Location location = CreateTestLocation();

            Department archivedDepartment = department.ChangeActivity(false);

            Assert.Throws<InvalidOperationException>(() => archivedDepartment.AddLocation(location));
        }

        [Fact]
        public void Should_Allow_SameLocation_InDifferentDepartments()
        {
            Department department1 = CreateTestDepartment();
            Department department2 = Department.CreateRoot(
                DepartmentName.Create("HR"),
                DepartmentIdentifier.Create("hr")
            );
            Location location = CreateTestLocation();

            department1.AddLocation(location);
            department2.AddLocation(location);

            Assert.Single(department1.Locations);
            Assert.Single(department2.Locations);
            Assert.Equal(location.Id, department1.Locations[0].LocationId);
            Assert.Equal(location.Id, department2.Locations[0].LocationId);
        }

        [Fact]
        public void Should_RemoveLocation()
        {
            Department department = CreateTestDepartment();
            Location location = CreateTestLocation();

            department.AddLocation(location);
            department.RemoveLocation(location.Id);

            Assert.Empty(department.Locations);
        }

        [Fact]
        public void Should_Throw_When_RemoveLocation_FromArchivedDepartment()
        {
            Department department = CreateTestDepartment();
            Location location = CreateTestLocation();

            department.AddLocation(location);

            Department archivedDepartment = department.ChangeActivity(false);

            Assert.Throws<InvalidOperationException>(() => archivedDepartment.RemoveLocation(location.Id));
        }

        [Fact]
        public void Should_Throw_When_RemoveLocation_NotExists()
        {
            Department department = CreateTestDepartment();
            LocationId fakeId = LocationId.Create();

            Assert.Throws<ArgumentException>(() => department.RemoveLocation(fakeId));
        }

        [Fact]
        public void Should_HaveLocation_WhenExists()
        {
            Department department = CreateTestDepartment();
            Location location = CreateTestLocation();

            department.AddLocation(location);

            Assert.True(department.HasLocation(location.Id));
        }

        [Fact]
        public void Should_NotHaveLocation_WhenNotExists()
        {
            Department department = CreateTestDepartment();
            LocationId fakeId = LocationId.Create();

            Assert.False(department.HasLocation(fakeId));
        }

        [Fact]
        public void Should_UpdateLifeTime_When_AddingPosition()
        {
            Department department = CreateTestDepartment();
            DateTime originalUpdatedAt = department.LifeTime.UpdatedAt;
            Thread.Sleep(10);

            Position position = CreateTestPosition("Developer");
            Rank rank = Rank.Create(5);
            department.AddPosition(position, rank);

            Assert.True(department.LifeTime.UpdatedAt > originalUpdatedAt);
        }

        [Fact]
        public void Should_UpdateLifeTime_When_AddingLocation()
        {
            Department department = CreateTestDepartment();
            DateTime originalUpdatedAt = department.LifeTime.UpdatedAt;
            Thread.Sleep(10);

            Location location = CreateTestLocation();
            department.AddLocation(location);

            Assert.True(department.LifeTime.UpdatedAt > originalUpdatedAt);
        }
        #endregion
    }
}