using Microsoft.AspNetCore.Mvc;
using System;

namespace Server.Api.Controllers
{
    /// <summary>
    /// Вспомогательный контроллер для получения текущего времени сервера (UTC).
    /// </summary>
    [ApiController]
    [Route("time")]
    public class TimeController : ControllerBase
    {
        /// <summary>
        /// Возвращает текущее серверное время в формате UTC.
        /// </summary>
        /// <returns>Значение <see cref="DateTime"/> в UTC.</returns>
        /// <response code="200">Время успешно возвращено.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DateTime))]
        public IActionResult GetCurrentTime() => Ok(DateTime.UtcNow);
    }
}
