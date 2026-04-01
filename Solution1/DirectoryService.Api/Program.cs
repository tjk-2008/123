using DirectoryService.Api;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Добавляем контроллеры
builder.Services.AddControllers();

// Добавляем Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Регистрируем хранилища как singletons
builder.Services.AddSingleton<LocationStorage>();
builder.Services.AddSingleton<PositionStorage>();

WebApplication app = builder.Build();

// Настройка Swagger (только в разработке)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

// Инициализация хранилищ тестовыми данными
LocationStorage locationStorage = app.Services.GetRequiredService<LocationStorage>();
PositionStorage positionStorage = app.Services.GetRequiredService<PositionStorage>();

locationStorage.InitializeStorage();
positionStorage.InitializeStorage();

app.Run();