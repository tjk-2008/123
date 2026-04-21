using DirectoryService.Domain.PositionsContext;
using DirectoryService.Domain.PositionsContext.ValueObjects;
using DirectoryService.Domain.Shared;
using MediatR;

namespace DirectoryService.Application.Commands.CreatePosition;

public class CreatePositionCommandHandler : IRequestHandler<CreatePositionCommand, Guid>
{
    public async Task<Guid> Handle(CreatePositionCommand request, CancellationToken cancellationToken)
    {
        // TODO: Добавить проверку уникальности через репозиторий
        // TODO: Сохранять в базу данных через репозиторий

        Position position = new Position(
            PositionId.Create(),
            PositionName.Create(request.Name),
            PositionDescription.Create(request.Description),
            true,
            EntityLifeTime.Create()
        );

        await Task.CompletedTask;

        return position.Id.Value;
    }
}