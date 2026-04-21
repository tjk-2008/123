using DirectoryService.Domain.LocationsContext;
using DirectoryService.Domain.LocationsContext.ValueObjects;
using DirectoryService.Domain.Shared;
using MediatR;

namespace DirectoryService.Application.Commands.CreateLocation;

public class CreateLocationCommandHandler : IRequestHandler<CreateLocationCommand, Guid>
{
    public async Task<Guid> Handle(CreateLocationCommand request, CancellationToken cancellationToken)
    {
        Location location = new Location(
            LocationId.Create(),
            LocationAddress.Create(request.Address),
            LocationName.Create(request.Name),
            IanaTimeZone.Create(request.TimeZone),
            EntityLifeTime.Create()
        );

        await Task.CompletedTask;

        return location.Id.Value;
    }
}