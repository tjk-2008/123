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
        #region ValueObject Tests
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

        [Theory]
        [InlineData("it"), InlineData("it.dev")]
        public void Should_CreatePath_When_Valid(string valid)
        {
            Assert.Equal(valid, DepartmentPath.Create(valid).Value);
        }

        [Fact]
        public void Should_CreateChildPath()
        {
            Assert.Equal("it.dev", DepartmentPath.CreateForChild(DepartmentPath.Create("it"), "dev").Value);
        }

        [Fact]
        public void Should_CalculateDepth()
        {
            Assert.Equal(3, DepartmentPath.Create("it.dev.backend").CalculateDepth());
        }
        #endregion

        #region Rank Tests
        [Theory]
        [InlineData(1), InlineData(50), InlineData(100)]
        public void Should_CreateRank_When_Valid(int value)
        {
            Assert.Equal(value, DepartmentPosition.Rank.Create(value).Value);
        }

        [Fact]
        public void Should_Throw_When_RankTooSmall()
        {
            Assert.Throws<ArgumentException>(() => DepartmentPosition.Rank.Create(0));
        }

        [Fact]
        public void Should_IncreaseRank()
        {
            Assert.Equal(9, DepartmentPosition.Rank.Create(10).Increase().Value);
        }

        [Fact]
        public void Should_DecreaseRank()
        {
            Assert.Equal(11, DepartmentPosition.Rank.Create(10).Decrease().Value);
        }
        #endregion

        #region Department Entity Tests
        private Department CreateTestDept()
        {
            return Department.CreateRoot(DepartmentName.Create("IT"), DepartmentIdentifier.Create("it"));
        }

        private Position CreateTestPos(string name)
        {
            return new(PositionId.Create(), PositionName.Create(name), PositionDescription.Create(""), true, EntityLifeTime.Create());
        }

        private Location CreateTestLoc()
        {
            return new(LocationId.Create(), LocationAddress.Create("ул. Ленина, 1"), LocationName.Create("Москва"),
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
            dept.AddPosition(CreateTestPos("Dev"), 5);
            Assert.Equal(5, dept.GetPositions()[0].PositionRank.Value);
        }

        [Fact]
        public void Should_Throw_When_DuplicateRank()
        {
            Department dept = CreateTestDept();
            dept.AddPosition(CreateTestPos("Dev"), 5);
            Assert.Throws<ArgumentException>(() => dept.AddPosition(CreateTestPos("Manager"), 5));
        }

        [Fact]
        public void Should_ChangePositionRank()
        {
            Department dept = CreateTestDept();
            Position pos = CreateTestPos("Dev");
            dept.AddPosition(pos, 5);
            dept.ChangePositionRank(pos.Id, 10);
            Assert.Equal(10, dept.GetPositions()[0].PositionRank.Value);
        }

        [Fact]
        public void Should_RemovePosition()
        {
            Department dept = CreateTestDept();
            Position pos = CreateTestPos("Dev");
            dept.AddPosition(pos, 5);
            dept.RemovePosition(pos.Id);
            Assert.Empty(dept.GetPositions());
        }

        [Fact]
        public void Should_AddLocation()
        {
            Department dept = CreateTestDept();
            dept.AddLocation(CreateTestLoc());
            Assert.Single(dept.GetLocations());
        }

        [Fact]
        public void Should_Throw_When_DuplicateLocation()
        {
            Department dept = CreateTestDept();
            Location loc = CreateTestLoc();
            dept.AddLocation(loc);
            Assert.Throws<ArgumentException>(() => dept.AddLocation(loc));
        }

        [Fact]
        public void Should_Allow_SameLocation_InDifferentDepartments()
        {
            Department dept1 = CreateTestDept();
            Department dept2 = Department.CreateRoot(DepartmentName.Create("HR"), DepartmentIdentifier.Create("hr"));
            Location loc = CreateTestLoc();
            dept1.AddLocation(loc);
            dept2.AddLocation(loc);
            Assert.Single(dept1.GetLocations());
            Assert.Single(dept2.GetLocations());
        }

        [Fact]
        public void Should_RemoveLocation()
        {
            Department dept = CreateTestDept();
            Location loc = CreateTestLoc();
            dept.AddLocation(loc);
            dept.RemoveLocation(loc.Id);
            Assert.Empty(dept.GetLocations());
        }

        [Fact]
        public void Should_HaveLocation_WhenExists()
        {
            Department dept = CreateTestDept();
            Location loc = CreateTestLoc();
            dept.AddLocation(loc);
            Assert.True(dept.HasLocation(loc.Id));
        }
        #endregion
    }
}