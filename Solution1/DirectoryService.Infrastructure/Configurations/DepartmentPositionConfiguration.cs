using DirectoryService.Domain.DepartmentsContext;
using DirectoryService.Domain.DepartmentsContext.ValueObjects;
using DirectoryService.Domain.PositionsContext.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Configurations
{
	public class DepartmentPositionConfiguration : IEntityTypeConfiguration<DepartmentPosition>
	{
		public void Configure(EntityTypeBuilder<DepartmentPosition> builder)
		{
			builder.ToTable("department_positions");

			builder.HasKey(dp => new { dp.DepartmentId, dp.PositionId });

			builder
				.Property(dp => dp.DepartmentId)
				.HasConversion(id => id.Value, value => DepartmentId.Create(value))
				.HasColumnName("department_id");

			builder
				.Property(dp => dp.PositionId)
				.HasConversion(id => id.Value, value => PositionId.Create(value))
				.HasColumnName("position_id");

			builder
				.Property(dp => dp.PositionRank)
				.HasColumnName("rank")
				.HasConversion(rank => rank.Value, value => Rank.Create(value));
		}
	}
}
