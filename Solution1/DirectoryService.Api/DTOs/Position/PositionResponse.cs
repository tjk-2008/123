namespace DirectoryService.Api.DTOs.Position;

public record PositionResponse
{
	public Guid Id { get; init; }
	public string Name { get; init; } = string.Empty;
	public string Description { get; init; } = string.Empty;
	public bool IsActive { get; init; }
	public DateTime CreatedAt { get; init; }
	public DateTime UpdatedAt { get; init; }

	public PositionResponse(
		Guid id,
		string name,
		string description,
		bool isActive,
		DateTime createdAt,
		DateTime updatedAt
	)
	{
		Id = id;
		Name = name;
		Description = description;
		IsActive = isActive;
		CreatedAt = createdAt;
		UpdatedAt = updatedAt;
	}
}
