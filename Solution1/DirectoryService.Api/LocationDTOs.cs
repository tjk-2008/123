namespace DirectoryService.Api;

public class CreateLocationRequest
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string TimeZone { get; set; } = string.Empty;
}

public class UpdateLocationRequest
{
    public string? Name { get; set; }
    public string? Address { get; set; }
    public string? TimeZone { get; set; }
}

public class LocationResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string TimeZone { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}