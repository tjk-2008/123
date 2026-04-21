using DirectoryService.Domain.PositionsContext;
using DirectoryService.Domain.PositionsContext.ValueObjects;
using DirectoryService.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Configurations
{
	public class PositionConfiguration : IEntityTypeConfiguration<Position>
	{
		public void Configure(EntityTypeBuilder<Position> builder)
		{
			builder.ToTable("positions");

			builder.HasKey(p => p.Id);
			builder
				.Property(p => p.Id)
				.HasConversion(id => id.Value, value => PositionId.Create(value))
				.HasColumnName("id");

			builder
				.Property(p => p.Name)
				.HasConversion(name => name.Value, value => PositionName.Create(value))
				.HasColumnName("position_name")
				.HasMaxLength(128)
				.IsRequired();

			builder
				.Property(p => p.Description)
				.HasConversion(desc => desc.Value, value => PositionDescription.Create(value))
				.HasColumnName("description")
				.HasColumnType("text")
				.IsRequired();

			builder.Property(p => p.IsActive).HasColumnName("is_active");

			builder
				.Property(p => p.LifeTime)
				.HasColumnName("life_time")
				.HasConversion(lt => $"{lt.CreatedAt}|{lt.UpdatedAt}|{lt.IsActive}", value => ParseLifeTime(value))
				.IsRequired();
		}

		private static EntityLifeTime ParseLifeTime(string value)
		{
			string[] parts = value.Split('|');
			return EntityLifeTime.Create(DateTime.Parse(parts[0]), DateTime.Parse(parts[1]), bool.Parse(parts[2]));
		}
	}
}
