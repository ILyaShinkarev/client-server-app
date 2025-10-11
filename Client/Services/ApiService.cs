using Newtonsoft.Json;

namespace Client.Services
{
    /// Сервис для работы с внешним API
    public class ApiService
    {
        private readonly HttpClient _http;

        public ApiService(IHttpClientFactory httpFactory) => _http = httpFactory.CreateClient("api");

        /// Получает список записей из API
        public async Task<List<Record>> GetRecordsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _http.GetAsync("/records", cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    // Десериализация JSON в список объектов
                    var records = JsonConvert.DeserializeObject<List<Record>>(json) ?? new();
                    Console.WriteLine($"Получено {records.Count} записей");
                    return records;
                }
                else
                {
                    Console.WriteLine($"Ошибка HTTP: {response.StatusCode}");
                    return new List<Record>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении записей: {ex.Message}");
                return new List<Record>();
            }
        }

        ///Получает время сервера через HTTP-заголовки
        public async Task<DateTime?> GetServerTimeAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                // Используем заголовок Date из HTTP-ответа как время сервера
                var response = await _http.GetAsync("/records", cancellationToken);
                return response.Headers.Date?.UtcDateTime ?? DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении времени: {ex.Message}");
                return DateTime.UtcNow;
            }
        }

        ///Модель данных записи
        public class Record
        {
            public int Id { get; set; }
            public DateTime DateCreated { get; set; }
            public string Message { get; set; } = string.Empty;
        }
    }
}