using DirectoryService.Domain.Contracts;
using DirectoryService.Domain.LocationsContext.ValueObjects;
using MediatR;

namespace DirectoryService.Application.Commands.UpdateLocation;

public class UpdateLocationCommandHandler : IRequestHandler<UpdateLocationCommand, Guid>
{
	private readonly ILocationRepository _repository;

	public UpdateLocationCommandHandler(ILocationRepository repository)
	{
		_repository = repository;
	}

	public async Task<Guid> Handle(UpdateLocationCommand command, CancellationToken cancellationToken)
	{
		// 1. Получаем сущность по Id
		var location = await _repository.GetById(command.Id, cancellationToken);
		if (location is null)
			throw new InvalidOperationException("Локация не найдена");

		// 2. Проверяем, что есть что обновлять
		if (command.Name is null && command.Address is null && command.TimeZone is null)
			throw new InvalidOperationException("Нет данных для обновления");

		// 3. Создаём Value Object'ы (если указаны)
		LocationName? newName = command.Name is not null ? LocationName.Create(command.Name) : null;
		LocationAddress? newAddress = command.Address is not null ? LocationAddress.Create(command.Address) : null;
		IanaTimeZone? newTimeZone = command.TimeZone is not null ? IanaTimeZone.Create(command.TimeZone) : null;

		// 4. Проверяем уникальность нового имени (если имя меняется)
		if (newName is not null)
		{
			var duplicate = await _repository.GetByName(newName, cancellationToken);
			if (duplicate is not null && duplicate.Id != location.Id)
				throw new InvalidOperationException($"Локация с названием '{newName.Value}' уже существует");
		}

		// 5. Вызываем доменную логику обновления
		location.Update(newName, newAddress, newTimeZone);

		// 6. Сохраняем изменения
		await _repository.Update(location, cancellationToken);

		return location.Id.Value;
	}
}
