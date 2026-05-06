using DirectoryService.Domain.DepartmentsContext.ValueObjects;
using DirectoryService.Domain.LocationsContext;
using DirectoryService.Domain.LocationsContext.ValueObjects;

namespace DirectoryService.Domain.DepartmentsContext
{
	public class DepartmentLocation
	{
		public DepartmentId DepartmentId { get; }
		public LocationId LocationId { get; }

		// Конструктор без параметров для EF Core
		private DepartmentLocation()
		{
			DepartmentId = null!;
			LocationId = null!;
		}

		public DepartmentLocation(DepartmentId departmentId, LocationId locationId)
		{
			DepartmentId = departmentId;
			LocationId = locationId;
		}
	}
}
