using MediatR;

namespace DirectoryService.Application.Commands.UpdatePosition;

public record UpdatePositionCommand(Guid Id, string Name) : IRequest<Guid>;
