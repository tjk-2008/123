using DirectoryService.Application.Handlers;
using DirectoryService.Domain.Contracts;
using DirectoryService.Infrastructure;
using DirectoryService.Infrastructure.Options;
using DirectoryService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Проверка наличия секции Database в appsettings.json
IConfigurationSection databaseSection = builder.Configuration.GetSection("Database");
if (!databaseSection.Exists())
{
	throw new InvalidOperationException("Секция 'Database' не найдена в appsettings.json");
}

// Регистрация DatabaseOptions с валидацией
builder.Services.AddOptions<DatabaseOptions>().Bind(databaseSection).ValidateDataAnnotations().ValidateOnStart();

// Регистрация DbContext и репозиториев
builder.Services.AddScoped<DirectoryDbContext>();
builder.Services.AddScoped<IPositionRepository, PositionRepository>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();

// Регистрация хендлеров (без MediatR)
builder.Services.AddScoped<PositionHandlers>();
builder.Services.AddScoped<LocationHandlers>();
builder.Services.AddScoped<DepartmentHandlers>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

WebApplication app = builder.Build();

// Проверка подключения к БД и применение миграций при старте
using (IServiceScope scope = app.Services.CreateScope())
{
	DirectoryDbContext dbContext = scope.ServiceProvider.GetRequiredService<DirectoryDbContext>();
	ILogger<Program> logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

	try
	{
		await dbContext.Database.CanConnectAsync();
		logger.LogInformation("Успешное подключение к базе данных");
		await dbContext.Database.MigrateAsync();
		logger.LogInformation("Миграции применены успешно");
	}
	catch (Exception ex)
	{
		logger.LogError(ex, "Не удалось подключиться к базе данных или применить миграции");
		throw;
	}
}

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
