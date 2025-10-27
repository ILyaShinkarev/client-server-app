using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Server.Api.Controllers
{
    /// <summary>
    /// Контроллер для получения списка записей из базы данных.
    /// </summary>
    [ApiController]
    [Route("records")]
    public class RecordsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<RecordsController> _logger;

        /// <summary>
        /// Создаёт экземпляр контроллера записей.
        /// </summary>
        /// <param name="db">Контекст базы данных приложения.</param>
        /// <param name="logger">Логгер для диагностики.</param>
        public RecordsController(ApplicationDbContext db, ILogger<RecordsController> logger)
        {
            _db = db;
            _logger = logger;
        }

        /// <summary>
        /// Возвращает список записей, опционально отфильтрованный по интервалу времени в UTC.
        /// </summary>
        /// <param name="from">
        /// Начало интервала (включительно), в UTC. Если не задано — нижняя граница не применяется.
        /// </param>
        /// <param name="to">
        /// Конец интервала (включительно), в UTC. Если не задано — верхняя граница не применяется.
        /// </param>
        /// <param name="ct">Токен отмены операции.</param>
        /// <returns>
        /// Объект <see cref="OkObjectResult"/> со списком записей, отсортированных по дате (по убыванию).
        /// </returns>
        /// <response code="200">Список записей успешно получен.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            CancellationToken ct)
        {
            var query = _db.Records.AsNoTracking();

            if (from.HasValue)
                query = query.Where(r => r.DateCreated >= from.Value);

            if (to.HasValue)
                query = query.Where(r => r.DateCreated <= to.Value);

            var list = await query
                .OrderByDescending(r => r.DateCreated)
                .ToListAsync(ct);

            return Ok(list);
        }
    }
}
