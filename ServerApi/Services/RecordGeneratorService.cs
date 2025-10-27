using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataAccess;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Server.Api.Services
{
    /// <summary>
    /// Фоновый сервис, который периодически (раз в минуту) создаёт тестовые записи в базе данных.
    /// </summary>
    public class RecordGeneratorService : BackgroundService
    {
        private readonly ILogger<RecordGeneratorService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        /// <summary>
        /// Создаёт экземпляр фонового сервиса.
        /// </summary>
        public RecordGeneratorService(IServiceScopeFactory scopeFactory, ILogger<RecordGeneratorService> logger)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        /// <summary>
        /// Каждую минуту создаёт новую запись в БД, пока не будет запрошена отмена.
        /// </summary>
        /// <param name="stoppingToken">Токен отмены, инициируемый при остановке хоста.</param>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var rec = new Record
                    {
                        DateCreated = DateTime.UtcNow,
                        Message = GenerateRandom()
                    };

                    await db.Records.AddAsync(rec, stoppingToken);
                    await db.SaveChangesAsync(stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    // Корректное завершение работы при отмене.
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при генерации тестовой записи.");
                }
            }
        }

        /// <summary>
        /// Генерирует случайную цифровую строку указанной длины.
        /// </summary>
        /// <param name="length">Длина строки; по умолчанию 10.</param>
        /// <returns>Строка, состоящая из случайных цифр.</returns>
        private static string GenerateRandom(int length = 10)
        {
            const string SYMBOLS = "0123456789";
            var rnd = new Random();
            return new string(Enumerable
                .Repeat(SYMBOLS, length)
                .Select(s => s[rnd.Next(s.Length)])
                .ToArray());
        }
    }
}
