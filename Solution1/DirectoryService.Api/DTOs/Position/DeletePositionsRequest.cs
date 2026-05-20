namespace DirectoryService.Api.DTOs.Position;

public record DeletePositionsRequest(IReadOnlyCollection<Guid> Ids);
