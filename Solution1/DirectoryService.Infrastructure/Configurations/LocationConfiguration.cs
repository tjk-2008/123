using System.Text.Json;
using DirectoryService.Domain.LocationsContext;
using DirectoryService.Domain.LocationsContext.ValueObjects;
using DirectoryService.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Configurations;

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

		// Упрощённая конфигурация для Address
		builder
			.Property(l => l.Address)
			.HasConversion(addr => addr.Value, value => LocationAddress.Create(value))
			.HasColumnName("location_address")
			.HasColumnType("text")
			.IsRequired();

		builder
			.Property(l => l.TimeZone)
			.HasConversion(tz => tz.Value, value => IanaTimeZone.Create(value))
			.HasColumnName("iana_time_zone")
			.HasMaxLength(255)
			.IsRequired();

		builder
			.Property(l => l.LifeTime)
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
