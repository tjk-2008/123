using DirectoryService.Domain.Shared;

namespace Tests
{
    public class UnitSharedTests
    {
        #region EntityLifeTime Tests
        [Fact]
        public void Should_Create_When_Default()
        {
            EntityLifeTime lifeTime = EntityLifeTime.Create();

            Assert.True(lifeTime.CreatedAt <= DateTime.UtcNow);
            Assert.Equal(lifeTime.CreatedAt, lifeTime.UpdatedAt);
            Assert.True(lifeTime.IsActive);
        }

        [Fact]
        public void Should_Create_When_ValidParams()
        {
            DateTime createdAt = DateTime.UtcNow.AddDays(-1);
            DateTime updatedAt = DateTime.UtcNow;

            EntityLifeTime lifeTime = EntityLifeTime.Create(createdAt, updatedAt);

            Assert.Equal(createdAt, lifeTime.CreatedAt);
            Assert.Equal(updatedAt, lifeTime.UpdatedAt);
            Assert.True(lifeTime.IsActive);
        }

        [Fact]
        public void Should_Throw_When_UpdatedAtLessThanCreatedAt()
        {
            DateTime createdAt = DateTime.UtcNow;
            DateTime updatedAt = DateTime.UtcNow.AddDays(-1);

            Assert.Throws<ArgumentException>(() =>
                EntityLifeTime.Create(createdAt, updatedAt));
        }

        [Fact]
        public void Should_Update_When_UpdateCalled()
        {
            EntityLifeTime lifeTime = EntityLifeTime.Create();
            Thread.Sleep(10);

            EntityLifeTime updated = lifeTime.Update();

            Assert.Equal(lifeTime.CreatedAt, updated.CreatedAt);
            Assert.True(updated.UpdatedAt > lifeTime.UpdatedAt);
            Assert.Equal(lifeTime.IsActive, updated.IsActive);
        }

        [Fact]
        public void Should_SetIsActiveFalse_When_Archive()
        {
            EntityLifeTime lifeTime = EntityLifeTime.Create();
            EntityLifeTime archived = lifeTime.Archive();

            Assert.False(archived.IsActive);
        }

        [Fact]
        public void Should_SetIsActiveTrue_When_Activate()
        {
            EntityLifeTime lifeTime = EntityLifeTime.Create(DateTime.UtcNow, DateTime.UtcNow, false);
            EntityLifeTime activated = lifeTime.Activate();

            Assert.True(activated.IsActive);
        }
        #endregion
    }
}