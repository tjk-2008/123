using DirectoryService.Domain.DepartmentsContext;
using DirectoryService.Domain.DepartmentsContext.ValueObjects;
using DirectoryService.Domain.LocationsContext;
using DirectoryService.Domain.LocationsContext.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Configurations
{
	public class DepartmentLocationConfiguration : IEntityTypeConfiguration<DepartmentLocation>
	{
		public void Configure(EntityTypeBuilder<DepartmentLocation> builder)
		{
			builder.ToTable("department_locations");

			// Составной первичный ключ
			builder.HasKey(dl => new { dl.DepartmentId, dl.LocationId });

			builder
				.Property(dl => dl.DepartmentId)
				.HasConversion(id => id.Value, value => DepartmentId.Create(value))
				.HasColumnName("department_id");

			builder
				.Property(dl => dl.LocationId)
				.HasConversion(id => id.Value, value => LocationId.Create(value))
				.HasColumnName("location_id");

			// Связь с Department
			builder
				.HasOne<Department>()
				.WithMany(d => d.Locations)
				.HasForeignKey(dl => dl.DepartmentId)
				.OnDelete(DeleteBehavior.Cascade);

			// Связь с Location
			builder.HasOne<Location>().WithMany().HasForeignKey(dl => dl.LocationId).OnDelete(DeleteBehavior.Cascade);
		}
	}
}
