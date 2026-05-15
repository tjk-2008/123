using DirectoryService.Domain.Contracts;
using DirectoryService.Domain.PositionsContext;
using DirectoryService.Domain.PositionsContext.ValueObjects;
using DirectoryService.Domain.Shared;

namespace DirectoryService.Application.Handlers;

public class PositionHandlers
{
	private readonly IPositionRepository _repository;

	public PositionHandlers(IPositionRepository repository)
	{
		_repository = repository;
	}

	// CREATE
	public async Task<Guid> CreatePosition(
		string name,
		string description,
		CancellationToken cancellationToken = default
	)
	{
		// Проверка уникальности
		bool exists = await _repository.IsNameUniqueAsync(name, cancellationToken);
		if (!exists)
		{
			throw new InvalidOperationException("Должность с таким названием уже существует");
		}

		Position position = new Position(
			PositionId.Create(),
			PositionName.Create(name),
			PositionDescription.Create(description),
			true,
			EntityLifeTime.Create()
		);

		await _repository.AddAsync(position, cancellationToken);
		return position.Id.Value;
	}

	// UPDATE
	public async Task<Guid> UpdatePosition(Guid id, string newName, CancellationToken cancellationToken = default)
	{
		Position? position =
			await _repository.GetById(id, cancellationToken)
			?? throw new InvalidOperationException("Должность не найдена");

		PositionName newPositionName = PositionName.Create(newName);

		// Проверка уникальности имени
		Position? duplicate = await _repository.GetByName(newPositionName, cancellationToken);
		if (duplicate is not null && duplicate.Id != position.Id)
		{
			throw new InvalidOperationException($"Должность с названием '{newName}' уже существует");
		}

		position.ChangePositionName(newPositionName);
		await _repository.Update(position, cancellationToken);

		return position.Id.Value;
	}

	// DELETE - массовое удаление
	public async Task<IReadOnlyCollection<Guid>> DeletePositions(
		IReadOnlyCollection<Guid> ids,
		CancellationToken cancellationToken = default
	)
	{
		List<PositionId> positionIds = ids.Select(PositionId.Create).ToList();
		IEnumerable<Position> positions = await _repository.GetManyByIds(positionIds, cancellationToken);
		List<Position> positionsList = positions.ToList();

		if (positionsList.Count != ids.Count)
		{
			throw new InvalidOperationException("Некоторые должности не найдены");
		}

		foreach (Position position in positionsList)
		{
			await _repository.Delete(position, cancellationToken);
		}

		return positionsList.Select(p => p.Id.Value).ToList();
	}
}
