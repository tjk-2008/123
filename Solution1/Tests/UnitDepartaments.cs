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
        public void Should_Create_When_ValidDepth(short valid)
        {
            DepartmentDepth depth = DepartmentDepth.Create(valid);
            Assert.Equal(valid, depth.Value);
        }

        [Fact]
        public void Should_Throw_When_DepthIsNegative()
        {
            Assert.Throws<ArgumentException>(() => DepartmentDepth.Create(-1));
        }

        [Fact]
        public void Should_Increase_When_Increment()
        {
            DepartmentDepth depth = DepartmentDepth.Create(5);
            Assert.Equal(6, depth.Increment().Value);
        }
        #endregion

        #region DepartmentId Tests
        [Fact]
        public void Should_Create_When_NewGuid()
        {
            Assert.NotEqual(DepartmentId.Create(), DepartmentId.Create());
        }

        [Fact]
        public void Should_Throw_When_GuidIsEmpty()
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
        [InlineData("it")]
        [InlineData("hr")]
        public void Should_Create_When_ValidIdentifier(string valid)
        {
            DepartmentIdentifier id = DepartmentIdentifier.Create(valid);
            Assert.Equal(valid, id.Value);
        }

        [Theory]
        [InlineData("")]
        [InlineData("IT")]
        public void Should_Throw_When_InvalidIdentifier(string invalid)
        {
            Assert.Throws<ArgumentException>(() => DepartmentIdentifier.Create(invalid));
        }
        #endregion

        #region DepartmentName Tests
        [Theory]
        [InlineData("IT")]
        [InlineData("Sales")]
        public void Should_Create_When_ValidName(string valid)
        {
            DepartmentName name = DepartmentName.Create(valid);
            Assert.Equal(valid, name.Value);
        }

        [Theory]
        [InlineData("")]
        [InlineData("A")]
        public void Should_Throw_When_InvalidName(string invalid)
        {
            Assert.Throws<ArgumentException>(() => DepartmentName.Create(invalid));
        }
        #endregion

        #region DepartmentPath Tests
        [Theory]
        [InlineData("it")]
        [InlineData("it.dev")]
        public void Should_Create_When_ValidPath(string valid)
        {
            DepartmentPath path = DepartmentPath.Create(valid);
            Assert.Equal(valid, path.Value);
        }

        [Theory]
        [InlineData("")]
        [InlineData("it..dev")]
        public void Should_Throw_When_InvalidPath(string invalid)
        {
            Assert.Throws<ArgumentException>(() => DepartmentPath.Create(invalid));
        }

        [Fact]
        public void Should_CreateChildPath_When_ValidParent() // переименовано
        {
            DepartmentPath parent = DepartmentPath.Create("it");
            DepartmentPath child = DepartmentPath.CreateForChild(parent, "dev");

            Assert.Equal("it.dev", child.Value);
        }

        [Fact]
        public void Should_ReturnCorrectDepth_When_CalculateDepth()
        {
            DepartmentPath path = DepartmentPath.Create("it.dev.backend");

            Assert.Equal(3, path.CalculateDepth());
        }
        #endregion

        #region Department Entity Tests
        private Department CreateTestDepartment()
        {
            return Department.CreateRoot(
                DepartmentName.Create("IT"),
                DepartmentIdentifier.Create("it")
            );
        }

        private Position CreateTestPosition(string name)
        {
            return new Position(
                PositionId.Create(),
                PositionName.Create(name),
                PositionDescription.Create(""),
                true,
                EntityLifeTime.Create()
            );
        }

        [Fact]
        public void Should_CreateRootDepartment_When_ValidData() // переименовано
        {
            Department dept = Department.CreateRoot(
                DepartmentName.Create("IT"),
                DepartmentIdentifier.Create("it")
            );

            Assert.Equal("it", dept.Path.Value);
            Assert.Equal(1, dept.Depth.Value);
            Assert.Null(dept.ParentId);
        }

        [Fact]
        public void Should_CreateChildDepartment_When_ValidParent() // переименовано
        {
            Department parent = CreateTestDepartment();

            Department child = Department.CreateChild(
                DepartmentName.Create("Dev"),
                DepartmentIdentifier.Create("dev"),
                parent
            );

            Assert.Equal("it.dev", child.Path.Value);
            Assert.Equal(parent.Id, child.ParentId);
        }

        [Fact]
        public void Should_Throw_When_AddingDuplicatePosition()
        {
            Department dept = CreateTestDepartment();

            Position pos1 = CreateTestPosition("Developer");
            Position pos2 = CreateTestPosition("Developer");

            dept.AddPosition(pos1);

            Assert.Throws<ArgumentException>(() => dept.AddPosition(pos2));
        }

        [Fact]
        public void Should_Throw_When_AddingDuplicateOffice()
        {
            Department dept = CreateTestDepartment();

            Location office = new Location(
                LocationId.Create(),
                LocationAddress.Create("ул. Ленина, 1"),
                LocationName.Create("Москва"),
                IanaTimeZone.Create("Europe/Moscow"),
                EntityLifeTime.Create()
            );

            dept.Addoffice(office);

            Assert.Throws<ArgumentException>(() => dept.Addoffice(office));
        }
        #endregion
    }
}