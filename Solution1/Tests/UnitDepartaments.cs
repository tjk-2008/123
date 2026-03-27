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
        [InlineData(0), InlineData(5)]
        public void Should_CreateDepth_When_Valid(short valid)
        {
            Assert.Equal(valid, DepartmentDepth.Create(valid).Value);
        }

        [Fact]
        public void Should_Throw_When_DepthNegative()
        {
            Assert.Throws<ArgumentException>(() => DepartmentDepth.Create(-1));
        }

        [Fact]
        public void Should_IncreaseDepth()
        {
            Assert.Equal(6, DepartmentDepth.Create(5).Increment().Value);
        }
        #endregion

        #region DepartmentId Tests
        [Fact]
        public void Should_CreateId_When_NewGuid()
        {
            Assert.NotEqual(DepartmentId.Create(), DepartmentId.Create());
        }

        [Fact]
        public void Should_Throw_When_IdEmpty()
        {
            Assert.Throws<ArgumentException>(() => DepartmentId.Create(Guid.Empty));
        }

        [Fact]
        public void Should_ReturnNull_When_CreateNullableWithNull()
        {
            Assert.Null(DepartmentId.CreateNullable(null));
        }
        #endregion

        #region DepartmentIdentifier Tests
        [Theory]
        [InlineData("it"), InlineData("hr")]
        public void Should_CreateIdentifier_When_Valid(string valid)
        {
            Assert.Equal(valid, DepartmentIdentifier.Create(valid).Value);
        }

        [Theory]
        [InlineData(""), InlineData("IT")]
        public void Should_Throw_When_IdentifierInvalid(string invalid)
        {
            Assert.Throws<ArgumentException>(() => DepartmentIdentifier.Create(invalid));
        }
        #endregion

        #region DepartmentName Tests
        [Theory]
        [InlineData("IT"), InlineData("Sales")]
        public void Should_CreateName_When_Valid(string valid)
        {
            Assert.Equal(valid, DepartmentName.Create(valid).Value);
        }

        [Theory]
        [InlineData(""), InlineData("A")]
        public void Should_Throw_When_NameInvalid(string invalid)
        {
            Assert.Throws<ArgumentException>(() => DepartmentName.Create(invalid));
        }
        #endregion

        #region DepartmentPath Tests
        [Theory]
        [InlineData("it"), InlineData("it.dev")]
        public void Should_CreatePath_When_Valid(string valid)
        {
            Assert.Equal(valid, DepartmentPath.Create(valid).Value);
        }

        [Theory]
        [InlineData(""), InlineData("it..dev")]
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
            Assert.Equal(3, path.CalculateDepth());
        }
        #endregion

        #region Rank Tests
        [Theory]
        [InlineData(1), InlineData(50), InlineData(100)]
        public void Should_CreateRank_When_Valid(int value)
        {
            Assert.Equal(value, Rank.Create(value).Value);
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
            Assert.Equal(11, increased.Value);
        }

        [Fact]
        public void Should_Throw_When_IncreaseMaxRank()
        {
            Assert.Throws<InvalidOperationException>(() => Rank.Create(100).Increase());
        }

        [Fact]
        public void Should_DecreaseRank()
        {
            Rank rank = Rank.Create(10);
            Rank decreased = rank.Decrease();
            Assert.Equal(9, decreased.Value);
        }

        [Fact]
        public void Should_Throw_When_DecreaseMinRank()
        {
            Assert.Throws<InvalidOperationException>(() => Rank.Create(1).Decrease());
        }
        #endregion

        #region Department Entity Tests
        private Department CreateTestDept()
        {
            return Department.CreateRoot(DepartmentName.Create("IT"), DepartmentIdentifier.Create("it"));
        }

        private Position CreateTestPos(string name)
        {
            return new Position(PositionId.Create(), PositionName.Create(name), PositionDescription.Create(""), true, EntityLifeTime.Create());
        }

        private Location CreateTestLoc()
        {
            return new Location(LocationId.Create(), LocationAddress.Create("ул. Ленина, 1"), LocationName.Create("Москва"),
                IanaTimeZone.Create("Europe/Moscow"), EntityLifeTime.Create());
        }

        [Fact]
        public void Should_CreateRoot()
        {
            Assert.True(CreateTestDept().IsRoot());
        }

        [Fact]
        public void Should_CreateChild()
        {
            Department parent = CreateTestDept();
            Department child = Department.CreateChild(DepartmentName.Create("Dev"), DepartmentIdentifier.Create("dev"), parent);
            Assert.Equal(parent.Id, child.ParentId);
        }

        [Fact]
        public void Should_AddPosition_WithRank()
        {
            Department dept = CreateTestDept();
            Position position = CreateTestPos("Dev");
            Rank rank = Rank.Create(5);

            dept.AddPosition(position, rank);

            Assert.Equal(5, dept.Positions[0].PositionRank.Value);
        }

        [Fact]
        public void Should_Throw_When_AddingDuplicatePosition()
        {
            Department dept = CreateTestDept();
            Position position = CreateTestPos("Dev");
            Rank rank = Rank.Create(5);

            dept.AddPosition(position, rank);

            Assert.Throws<ArgumentException>(() => dept.AddPosition(position, Rank.Create(10)));
        }

        [Fact]
        public void Should_ChangePositionRank()
        {
            Department dept = CreateTestDept();
            Position position = CreateTestPos("Dev");
            Rank oldRank = Rank.Create(5);
            Rank newRank = Rank.Create(10);

            dept.AddPosition(position, oldRank);
            dept.ChangePositionRank(position.Id, newRank);

            Assert.Equal(newRank, dept.Positions[0].PositionRank);
        }

        [Fact]
        public void Should_RemovePosition()
        {
            Department dept = CreateTestDept();
            Position position = CreateTestPos("Dev");
            Rank rank = Rank.Create(5);

            dept.AddPosition(position, rank);
            dept.RemovePosition(position.Id);

            Assert.Empty(dept.Positions);
        }

        [Fact]
        public void Should_AddLocation()
        {
            Department dept = CreateTestDept();
            Location location = CreateTestLoc();

            dept.AddLocation(location);

            Assert.Single(dept.Locations);
        }

        [Fact]
        public void Should_Throw_When_AddingDuplicateLocation()
        {
            Department dept = CreateTestDept();
            Location location = CreateTestLoc();

            dept.AddLocation(location);

            Assert.Throws<ArgumentException>(() => dept.AddLocation(location));
        }

        [Fact]
        public void Should_Allow_SameLocation_InDifferentDepartments()
        {
            Department dept1 = CreateTestDept();
            Department dept2 = Department.CreateRoot(DepartmentName.Create("HR"), DepartmentIdentifier.Create("hr"));
            Location location = CreateTestLoc();

            dept1.AddLocation(location);
            dept2.AddLocation(location);

            Assert.Single(dept1.Locations);
            Assert.Single(dept2.Locations);
        }

        [Fact]
        public void Should_RemoveLocation()
        {
            Department dept = CreateTestDept();
            Location location = CreateTestLoc();

            dept.AddLocation(location);
            dept.RemoveLocation(location.Id);

            Assert.Empty(dept.Locations);
        }

        [Fact]
        public void Should_HaveLocation_WhenExists()
        {
            Department dept = CreateTestDept();
            Location location = CreateTestLoc();

            dept.AddLocation(location);

            Assert.True(dept.HasLocation(location.Id));
        }

        [Fact]
        public void Should_Throw_When_AddPosition_ToArchivedDepartment()
        {
            Department dept = CreateTestDept();
            Department archived = dept.ChangeActivity(false);

            Assert.Throws<InvalidOperationException>(() => archived.AddPosition(CreateTestPos("Dev"), Rank.Create(5)));
        }

        [Fact]
        public void Should_Throw_When_AddLocation_ToArchivedDepartment()
        {
            Department dept = CreateTestDept();
            Department archived = dept.ChangeActivity(false);

            Assert.Throws<InvalidOperationException>(() => archived.AddLocation(CreateTestLoc()));
        }
        #endregion
    }
}