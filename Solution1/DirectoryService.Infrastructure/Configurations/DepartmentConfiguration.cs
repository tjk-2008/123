using DirectoryService.Domain.DepartmentsContext;
using DirectoryService.Domain.DepartmentsContext.ValueObjects;
using DirectoryService.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Configurations
{
	public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
	{
		public void Configure(EntityTypeBuilder<Department> builder)
		{
			builder.ToTable("departments");

			builder.HasKey(d => d.Id);
			builder
				.Property(d => d.Id)
				.HasConversion(id => id.Value, value => DepartmentId.Create(value))
				.HasColumnName("id");

			builder
				.Property(d => d.Name)
				.HasConversion(name => name.Value, value => DepartmentName.Create(value))
				.HasColumnName("department_name")
				.HasMaxLength(128)
				.IsRequired();

			builder
				.Property(d => d.Identifier)
				.HasConversion(id => id.Value, value => DepartmentIdentifier.Create(value))
				.HasColumnName("department_identifier")
				.HasMaxLength(50)
				.IsRequired();

			builder
				.Property(d => d.ParentId)
				.HasConversion(
					id => id != null ? id.Value : Guid.Empty,
					value => value != Guid.Empty ? DepartmentId.Create(value) : null
				)
				.HasColumnName("parent_id");

			builder
				.Property(d => d.Path)
				.HasConversion(path => path.Value, value => DepartmentPath.Create(value))
				.HasColumnName("department_path")
				.IsRequired();

			builder
				.Property(d => d.Depth)
				.HasConversion(depth => depth.Value, value => DepartmentDepth.Create(value))
				.HasColumnName("department_depth")
				.IsRequired();

			builder.Property(d => d.IsActive).HasColumnName("is_active");

			builder
				.Property(d => d.LifeTime)
				.HasColumnName("life_time")
				.HasConversion(lt => $"{lt.CreatedAt}|{lt.UpdatedAt}|{lt.IsActive}", value => ParseLifeTime(value))
				.IsRequired();

			builder.Ignore(d => d.Positions);
			builder.Ignore(d => d.Locations);
		}

		private static EntityLifeTime ParseLifeTime(string value)
		{
			string[] parts = value.Split('|');
			return EntityLifeTime.Create(DateTime.Parse(parts[0]), DateTime.Parse(parts[1]), bool.Parse(parts[2]));
		}
	}
}
