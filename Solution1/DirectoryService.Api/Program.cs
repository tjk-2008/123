using DirectoryService.Application;
using DirectoryService.Domain.Contracts;
using DirectoryService.Infrastructure;
using DirectoryService.Infrastructure.Options;
using DirectoryService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Настройка DatabaseOptions
DatabaseOptions databaseOptions =
	builder.Configuration.GetSection("Database").Get<DatabaseOptions>() ?? new DatabaseOptions();
builder.Services.AddSingleton(databaseOptions);

// Регистрация DbContext
builder.Services.AddDbContext<DirectoryDbContext>(options => options.UseNpgsql(databaseOptions.GetConnectionString()));

// Регистрация репозиториев
builder.Services.AddScoped<IPositionRepository, PositionRepository>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();

// Регистрация Application слоя
builder.Services.AddApplication();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

WebApplication app = builder.Build();

// Применение миграций при старте
using (IServiceScope scope = app.Services.CreateScope())
{
	DirectoryDbContext dbContext = scope.ServiceProvider.GetRequiredService<DirectoryDbContext>();
	await dbContext.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
