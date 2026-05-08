using DirectoryService.Domain.Contracts;
using DirectoryService.Domain.PositionsContext;
using DirectoryService.Domain.PositionsContext.ValueObjects;
using DirectoryService.Domain.Shared;
using MediatR;

namespace DirectoryService.Application.Commands.CreatePosition;

public class CreatePositionCommandHandler(IPositionRepository repository) : IRequestHandler<CreatePositionCommand, Guid>
{
	public async Task<Guid> Handle(CreatePositionCommand request, CancellationToken cancellationToken)
	{
		if (!await repository.IsNameUniqueAsync(request.Name, cancellationToken))
		{
			throw new InvalidOperationException("Должность с таким названием уже существует");
		}

		Position position = new(
			PositionId.Create(),
			PositionName.Create(request.Name),
			PositionDescription.Create(request.Description),
			true,
			EntityLifeTime.Create()
		);

		await repository.AddAsync(position, cancellationToken);

		return position.Id.Value;
	}
}
