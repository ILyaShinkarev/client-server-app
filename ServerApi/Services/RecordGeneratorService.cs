using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Server.Api.Data;
using Server.Api.Data.Models;

namespace Server.Api.Serviсes
{
    ///Фоновый сервис для генерации тестовых записей
    public class RecordGeneratorService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public RecordGeneratorService(IServiceScopeFactory scopeFactory, ILogger<RecordGeneratorService> logger)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Таймер для периодического выполнения каждую минуту
            using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromMinutes(1));

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    // Создаем новую область для каждого выполнения
                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    // Создаем и сохраняем новую запись
                    var rec = new Record() { DateCreated = DateTime.UtcNow, Message = GenerateRandom() };
                    await db.Records.AddAsync(rec, stoppingToken);
                    await db.SaveChangesAsync(stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    // Корректная обработка отмены операции
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while generating record");
                }
            }
        }

        ///Генерирует случайную строку из цифр
        private string GenerateRandom(int length = 10)
        {
            const string SYMBOL = "0123456789";
            var rnd = new Random();
            var message = new string(Enumerable.Repeat(SYMBOL, length)
                .Select(s => s[rnd.Next(s.Length)])
                .ToArray());
            return message;
        }
    }
}