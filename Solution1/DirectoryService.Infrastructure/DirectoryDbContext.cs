using DirectoryService.Domain.DepartmentsContext;
using DirectoryService.Domain.LocationsContext;
using DirectoryService.Domain.PositionsContext;
using DirectoryService.Infrastructure.Configurations;
using DirectoryService.Infrastructure.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DirectoryService.Infrastructure
{
	public class DirectoryDbContext : DbContext
	{
		private readonly DatabaseOptions _options;

		// Конструктор для DI (через IOptions)
		public DirectoryDbContext(IOptions<DatabaseOptions> options)
		{
			_options =
				options.Value
				?? throw new InvalidOperationException("DatabaseOptions не зарегистрированы в DI контейнере");
		}

		// Конструктор без параметров ТОЛЬКО для миграций (Design Time)
		public DirectoryDbContext()
		{
			_options = new DatabaseOptions
			{
				Host = "localhost",
				Port = 5432,
				Database = "directory_service",
				Username = "postgres",
				Password = "postgres",
			};
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
				if (_options == null)
				{
					throw new InvalidOperationException("DatabaseOptions не инициализированы");
				}

				optionsBuilder.UseNpgsql(_options.GetConnectionString());
			}
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// Сначала применяем конфигурации связующих таблиц
			modelBuilder.ApplyConfiguration(new DepartmentPositionConfiguration());
			modelBuilder.ApplyConfiguration(new DepartmentLocationConfiguration());

			// Затем конфигурации основных сущностей
			modelBuilder.ApplyConfiguration(new DepartmentConfiguration());
			modelBuilder.ApplyConfiguration(new PositionConfiguration());
			modelBuilder.ApplyConfiguration(new LocationConfiguration());
		}
	}
}
