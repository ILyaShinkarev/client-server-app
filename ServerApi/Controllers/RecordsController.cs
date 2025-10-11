using Microsoft.AspNetCore.Mvc;
using Server.Api.Data;
using Microsoft.EntityFrameworkCore;
using Server.Api.Serviсes;

namespace Server.Api.Controllers
{
    [ApiController]
    public class RecordsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<RecordsController> _logger;

        ///Конструктор с внедрением зависимостей
        public RecordsController(ApplicationDbContext db, ILogger<RecordsController> logger)
        {
            _db = db;
            _logger = logger;
        }

        ///Получение всех записей из БД
        [HttpGet("/records")]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            try
            {
                // Получаем записи без отслеживания изменений, отсортированные по дате
                var list = await _db.Records.AsNoTracking().OrderByDescending(x => x.DateCreated).ToListAsync(ct);
                return Ok(list);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                // Клиент отменил запрос
                return StatusCode(499);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get records");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}