namespace DirectoryService.Api.DTOs.Location;

public record LocationResponse
{
	public Guid Id { get; init; }
	public string Name { get; init; } = string.Empty;
	public string Address { get; init; } = string.Empty;
	public string TimeZone { get; init; } = string.Empty;
	public bool IsActive { get; init; }
	public DateTime CreatedAt { get; init; }
	public DateTime UpdatedAt { get; init; }

	public LocationResponse(
		Guid id,
		string name,
		string address,
		string timeZone,
		bool isActive,
		DateTime createdAt,
		DateTime updatedAt
	)
	{
		Id = id;
		Name = name;
		Address = address;
		TimeZone = timeZone;
		IsActive = isActive;
		CreatedAt = createdAt;
		UpdatedAt = updatedAt;
	}
}
