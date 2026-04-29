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

			// Используем ComplexProperty для LifeTime (как в PDF, листинг 31)
			builder.ComplexProperty(
				p => p.LifeTime,
				complexPropertyBuilder =>
				{
					complexPropertyBuilder.Property(lt => lt.CreatedAt).HasColumnName("created_at").IsRequired();

					complexPropertyBuilder.Property(lt => lt.UpdatedAt).HasColumnName("updated_at");

					complexPropertyBuilder.Property(lt => lt.IsActive).HasColumnName("is_active").IsRequired();
				}
			);
		}
	}
}
