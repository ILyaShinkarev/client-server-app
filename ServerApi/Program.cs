using Microsoft.EntityFrameworkCore;
using Server.Api.Data;
using Server.Api.Serviсes;
using System.Data.Common;

var builder = WebApplication.CreateBuilder(args);

// Регистрация сервисов в контейнере DI
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Настройка Entity Framework с SQLite
builder.Services.AddDbContext<ApplicationDbContext>(opt => opt.UseSqlite("Data Source=app.db"));

// Регистрация фонового сервиса
builder.Services.AddHostedService<RecordGeneratorService>();

var app = builder.Build();

// Автоматическое применение миграций БД при запуске
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

// Настройка pipeline запросов для development-окружения
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();