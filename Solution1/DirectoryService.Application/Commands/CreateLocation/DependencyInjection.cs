using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.Application.Commands.CreateLocation;

public static class LocationServiceCollectionExtensions
{
	public static IServiceCollection AddLocationCommands(this IServiceCollection services)
	{
		services.AddMediatR(cfg =>
			cfg.RegisterServicesFromAssembly(typeof(LocationServiceCollectionExtensions).Assembly)
		);
		return services;
	}
}
