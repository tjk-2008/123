using DirectoryService.Application.Handlers;
using DirectoryService.Domain.Contracts;
using DirectoryService.Infrastructure;
using DirectoryService.Infrastructure.Options;
using DirectoryService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Настройка DatabaseOptions
var databaseOptions = builder.Configuration.GetSection("Database").Get<DatabaseOptions>()
    ?? throw new InvalidOperationException("Секция 'Database' не найдена");

builder.Services.AddSingleton(databaseOptions);

// Регистрация DbContext
builder.Services.AddScoped<DirectoryDbContext>();

// Регистрация репозиториев
builder.Services.AddScoped<IPositionRepository, PositionRepository>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();  // ← добавить

// Регистрация хендлеров
builder.Services.AddScoped<PositionHandlers>();
builder.Services.AddScoped<LocationHandlers>();
builder.Services.AddScoped<DepartmentHandlers>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Применение миграций
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DirectoryDbContext>();
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