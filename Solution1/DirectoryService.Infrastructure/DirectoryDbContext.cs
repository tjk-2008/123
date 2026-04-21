using DirectoryService.Domain.DepartmentsContext;
using DirectoryService.Domain.LocationsContext;
using DirectoryService.Domain.PositionsContext;
using DirectoryService.Infrastructure.Configurations;
using DirectoryService.Infrastructure.Options;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Infrastructure
{
	public class DirectoryDbContext : DbContext
	{
		private readonly DatabaseOptions? _options;

		public DirectoryDbContext(DatabaseOptions options)
		{
			_options = options;
		}

		public DirectoryDbContext()
		{
			_options = null;
		}

		public DbSet<Position> Positions { get; set; }
		public DbSet<Location> Locations { get; set; }
		public DbSet<Department> Departments { get; set; }
		public DbSet<DepartmentPosition> DepartmentPositions { get; set; }
		public DbSet<DepartmentLocation> DepartmentLocations { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (!optionsBuilder.IsConfigured)
			{
				if (_options != null)
				{
					string connectionString =
						$"Host={_options.Host};Port={_options.Port};Database={_options.Database};Username={_options.Username};Password={_options.Password}";
					optionsBuilder.UseNpgsql(connectionString);
				}
				else
				{
					optionsBuilder.UseNpgsql(
						"Host=localhost;Port=5432;Database=directory_service;Username=postgres;Password=postgres"
					);
				}
			}
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfiguration(new DepartmentConfiguration());
			modelBuilder.ApplyConfiguration(new DepartmentPositionConfiguration());
			modelBuilder.ApplyConfiguration(new DepartmentLocationConfiguration());
			modelBuilder.ApplyConfiguration(new PositionConfiguration());
			modelBuilder.ApplyConfiguration(new LocationConfiguration());
		}
	}
}
