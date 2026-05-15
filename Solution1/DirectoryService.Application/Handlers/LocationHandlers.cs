using DirectoryService.Domain.Contracts;
using DirectoryService.Domain.LocationsContext;
using DirectoryService.Domain.LocationsContext.ValueObjects;
using DirectoryService.Domain.Shared;

namespace DirectoryService.Application.Handlers;

public class LocationHandlers
{
	private readonly ILocationRepository _repository;

	public LocationHandlers(ILocationRepository repository)
	{
		_repository = repository;
	}

	// CREATE
	public async Task<Guid> CreateLocation(string name, string address, string timeZone, CancellationToken cancellationToken = default)
	{
		// Проверка уникальности
		bool exists = await _repository.IsNameUniqueAsync(name, cancellationToken);
		if (!exists)
			throw new InvalidOperationException("Локация с таким названием уже существует");

		Location location = new(
			LocationId.Create(),
			LocationAddress.Create(address),
			LocationName.Create(name),
			IanaTimeZone.Create(timeZone),
			EntityLifeTime.Create()
		);

		await _repository.AddAsync(location, cancellationToken);
		return location.Id.Value;
	}

	// UPDATE
	public async Task<Guid> UpdateLocation(Guid id, string? newName, string? newAddress, string? newTimeZone, CancellationToken cancellationToken = default)
	{
		Location? location = await _repository.GetById(id, cancellationToken);
		if (location is null)
			throw new InvalidOperationException("Локация не найдена");

		if (newName is null && newAddress is null && newTimeZone is null)
			throw new InvalidOperationException("Нет данных для обновления");

		// Проверка уникальности нового имени
		if (newName is not null)
		{
			LocationName newLocationName = LocationName.Create(newName);
			Location? duplicate = await _repository.GetByName(newLocationName, cancellationToken);
			if (duplicate is not null && duplicate.Id != location.Id)
				throw new InvalidOperationException($"Локация с названием '{newName}' уже существует");
		}

		location.Update(
			newName is not null ? LocationName.Create(newName) : null,
			newAddress is not null ? LocationAddress.Create(newAddress) : null,
			newTimeZone is not null ? IanaTimeZone.Create(newTimeZone) : null
		);

		await _repository.Update(location, cancellationToken);
		return location.Id.Value;
	}

	// DELETE
	public async Task<Guid> DeleteLocation(Guid id, CancellationToken cancellationToken = default)
	{
		Location? location = await _repository.GetById(id, cancellationToken);
		if (location is null)
			throw new InvalidOperationException("Локация не найдена");

		await _repository.Delete(location, cancellationToken);
		return location.Id.Value;
	}
}
