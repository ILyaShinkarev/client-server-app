using Newtonsoft.Json;
using System.Net.Http;

namespace Client.Services
{
    /// <summary>
    /// Клиент для вызова бэкенд-API из Blazor через обратный прокси (YARP).
    /// Ожидается, что запросы отправляются на относительные пути "api/…",
    /// а фронтенд (порт 5275) проксирует их на backend.
    /// </summary>
    public class ApiService
    {
        private readonly HttpClient _http;

        /// <summary>
        /// Создаёт экземпляр <see cref="ApiService"/>.
        /// </summary>
        /// <param name="httpFactory">
        /// Фабрика HTTP-клиентов. Должна предоставлять именованный клиент <c>"api"</c>,
        /// чей <see cref="HttpClient.BaseAddress"/> указывает на адрес фронтенда
        /// (например, <c>http://localhost:5275</c>), чтобы YARP перехватил путь <c>/api/**</c>.
        /// </param>
        public ApiService(IHttpClientFactory httpFactory) => _http = httpFactory.CreateClient("api");

        /// <summary>
        /// Возвращает список записей, опционально отфильтрованный по UTC-интервалу.
        /// </summary>
        /// <param name="fromUtc">Начало интервала (UTC), включительно. Если <c>null</c> — нижняя граница не применяется.</param>
        /// <param name="toUtc">Конец интервала (UTC), включительно. Если <c>null</c> — верхняя граница не применяется.</param>
        /// <param name="ct">Токен отмены.</param>
        /// <returns>Список записей; пустой список, если ничего не найдено.</returns>
        /// <exception cref="HttpRequestException">Если сервер вернул код ошибки HTTP.</exception>
        public async Task<List<Record>> GetRecordsAsync(DateTime? fromUtc = null, DateTime? toUtc = null, CancellationToken ct = default)
        {
            var qs = new List<string>();
            if (fromUtc.HasValue) qs.Add("from=" + Uri.EscapeDataString(fromUtc.Value.ToString("o"))); // ISO-8601 UTC
            if (toUtc.HasValue) qs.Add("to=" + Uri.EscapeDataString(toUtc.Value.ToString("o")));

            var url = "api/records" + (qs.Count > 0 ? "?" + string.Join("&", qs) : "");

            var resp = await _http.GetAsync(url, ct);
            resp.EnsureSuccessStatusCode();

            var json = await resp.Content.ReadAsStringAsync(ct);
            return JsonConvert.DeserializeObject<List<Record>>(json) ?? new();
        }

        /// <summary>
        /// Возвращает текущее серверное время (UTC) через прокси.
        /// </summary>
        /// <param name="ct">Токен отмены.</param>
        /// <returns>Текущее UTC-время; <c>null</c>, если сервер вернул ошибку.</returns>
        public async Task<DateTime?> GetServerTimeAsync(CancellationToken ct = default)
        {
            var resp = await _http.GetAsync("api/time", ct); 
            if (!resp.IsSuccessStatusCode) return null;

            var json = await resp.Content.ReadAsStringAsync(ct);
            return JsonConvert.DeserializeObject<DateTime>(json);
        }

        /// <summary>
        /// DTO записи, соответствующий контракту API.
        /// </summary>
        public class Record
        {
            /// <summary>Идентификатор записи.</summary>
            public int Id { get; set; }

            /// <summary>Дата создания записи (UTC).</summary>
            public DateTime DateCreated { get; set; }

            /// <summary>Строка с данными размеров 10 символов.</summary>
            public string Message { get; set; } = string.Empty;
        }
    }
}
