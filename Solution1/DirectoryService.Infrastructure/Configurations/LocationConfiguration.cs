using System.Text.Json;
using DirectoryService.Domain.DepartmentsContext;
using DirectoryService.Domain.LocationsContext;
using DirectoryService.Domain.LocationsContext.ValueObjects;
using DirectoryService.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Configurations
{
	public class LocationConfiguration : IEntityTypeConfiguration<Location>
	{
		public void Configure(EntityTypeBuilder<Location> builder)
		{
			builder.ToTable("locations");

			builder.HasKey(l => l.Id);
			builder
				.Property(l => l.Id)
				.HasConversion(id => id.Value, value => LocationId.Create(value))
				.HasColumnName("id");

			builder
				.Property(l => l.Name)
				.HasConversion(name => name.Value, value => LocationName.Create(value))
				.HasColumnName("location_name")
				.HasMaxLength(128)
				.IsRequired();

			builder
				.Property(l => l.Address)
				.HasConversion(
					addr => JsonSerializer.Serialize(addr.Value),
					value => LocationAddress.Create(JsonSerializer.Deserialize<string>(value) ?? string.Empty)
				)
				.HasColumnName("location_address")
				.HasColumnType("jsonb")
				.IsRequired();

			builder
				.Property(l => l.TimeZone)
				.HasConversion(tz => tz.Value, value => IanaTimeZone.Create(value))
				.HasColumnName("iana_time_zone")
				.HasMaxLength(255)
				.IsRequired();

			builder.ComplexProperty(
				l => l.LifeTime,
				complexPropertyBuilder =>
				{
					complexPropertyBuilder.Property(lt => lt.CreatedAt).HasColumnName("created_at").IsRequired();
					complexPropertyBuilder.Property(lt => lt.UpdatedAt).HasColumnName("updated_at");
					complexPropertyBuilder.Property(lt => lt.IsActive).HasColumnName("is_active").IsRequired();
				}
			);

			builder
				.HasMany<DepartmentLocation>()
				.WithOne()
				.HasForeignKey(dl => dl.LocationId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
