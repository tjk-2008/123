using DirectoryService.Domain.Contracts;
using DirectoryService.Domain.PositionsContext.ValueObjects;
using MediatR;

namespace DirectoryService.Application.Commands.UpdatePosition;

public class UpdatePositionCommandHandler : IRequestHandler<UpdatePositionCommand, Guid>
{
	private readonly IPositionRepository _repository;

	public UpdatePositionCommandHandler(IPositionRepository repository)
	{
		_repository = repository;
	}

	public async Task<Guid> Handle(UpdatePositionCommand command, CancellationToken cancellationToken)
	{
		// 1. Получаем сущность по Id
		var position = await _repository.GetById(command.Id, cancellationToken);
		if (position is null)
			throw new InvalidOperationException("Должность не найдена");

		// 2. Проверяем, что новое имя не занято другим объектом
		var newName = PositionName.Create(command.Name);
		var duplicate = await _repository.GetByName(newName, cancellationToken);
		if (duplicate is not null && duplicate.Id != position.Id)
			throw new InvalidOperationException($"Должность с названием '{newName.Value}' уже существует");

		// 3. Вызываем доменную логику изменения имени
		position.ChangePositionName(newName);

		// 4. Сохраняем изменения
		await _repository.Update(position, cancellationToken);

		return position.Id.Value;
	}
}
