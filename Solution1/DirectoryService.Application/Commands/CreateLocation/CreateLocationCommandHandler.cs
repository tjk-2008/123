using DirectoryService.Domain.Contracts;
using DirectoryService.Domain.LocationsContext;
using DirectoryService.Domain.LocationsContext.ValueObjects;
using DirectoryService.Domain.Shared;
using MediatR;

namespace DirectoryService.Application.Commands.CreateLocation;

public class CreateLocationCommandHandler(ILocationRepository repository) : IRequestHandler<CreateLocationCommand, Guid>
{
	public async Task<Guid> Handle(CreateLocationCommand request, CancellationToken cancellationToken)
	{
		if (!await repository.IsNameUniqueAsync(request.Name, cancellationToken))
		{
			throw new InvalidOperationException("Локация с таким названием уже существует");
		}

		Location location = new(
			LocationId.Create(Guid.NewGuid()),
			LocationAddress.Create(request.Address),
			LocationName.Create(request.Name),
			IanaTimeZone.Create(request.TimeZone),
			EntityLifeTime.Create()
		);

		await repository.AddAsync(location, cancellationToken);

		return location.Id.Value;
	}
}
