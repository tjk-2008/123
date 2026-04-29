using MediatR;

namespace DirectoryService.Application.Commands.CreateLocation;

public record CreateLocationCommand(string Name, string Address, string TimeZone) : IRequest<Guid>;
