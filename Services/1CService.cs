using Microsoft.Net.Http.Headers;
using System.Net.Http;
using System.Net.Http.Json;

namespace WebRegistry.Services
{
    /// <summary>
    /// Класс, что забирает данные с 1С по веб-апи
    /// </summary>
    public class _1CService
    {
        private const string endpoint = "http://192.168.2.37/ErpExchange/hs/APIerp/V1/nomlist";
        private readonly HttpClient _httpClient;

        public _1CService()
        {
            _httpClient = new();
            _httpClient.BaseAddress = new Uri(endpoint);
        }

        public _1CService(HttpClient httpClient)
        {
            _httpClient = httpClient;

           // _httpClient.BaseAddress = new Uri(System.Configuration.ConfigurationManager.AppSettings["_1CEndPoint"] ?? throw new InvalidOperationException("Не могу найти end-point 1С в appsettings.json"));
             _httpClient.BaseAddress = new Uri(endpoint);
        }

        public async Task<IEnumerable<Card_1C_DTO>?> GetAll() =>
            await _httpClient.GetFromJsonAsync<IEnumerable<Card_1C_DTO>>("");

        public async Task<IEnumerable<Card_1C_DTO>?> GetById(int id) =>
            await _httpClient.GetFromJsonAsync<IEnumerable<Card_1C_DTO>>($"nomenclature?id={id}");

        public async Task<IEnumerable<Card_1C_DTO>?> GetByName(string name) =>
            await _httpClient.GetFromJsonAsync<IEnumerable<Card_1C_DTO>>($"nomenclature?articul={name}");

        public async Task<IEnumerable<Card_1C_DTO>?> GetByArt(string articul) =>
            await _httpClient.GetFromJsonAsync<IEnumerable<Card_1C_DTO>>($"nomenclature?articul={articul}");

    }
}

public class Card_1C_DTO
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Articul { get; set; }
    public bool IsArc { get; set; }
}
