using MediatR;

namespace DirectoryService.Application.Commands.UpdateLocation;

public record UpdateLocationCommand(Guid Id, string? Name, string? Address, string? TimeZone) : IRequest<Guid>;
