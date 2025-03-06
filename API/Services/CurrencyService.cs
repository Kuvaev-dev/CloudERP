using Domain.ServiceAccess;
using System.Text.Json;

namespace API.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly IConfiguration _configuration;
        private readonly string _apiUrl;

        private HttpClient _httpClient;

        public CurrencyService(IConfiguration configuration)
        {
            _configuration = configuration;
            _apiUrl = _configuration["CurrencyApi:BaseUrl"] ?? throw new ArgumentException("ExchangeRateApiUrl is not configured");
        }

        public async Task<Dictionary<string, decimal>> GetExchangeRatesAsync(string baseCurrency = "USD")
        {
            try
            {
                _httpClient = new HttpClient();

                var apiUrl = $"{_apiUrl}/latest/{baseCurrency}";
                var response = await _httpClient.GetAsync(apiUrl);

                if (!response.IsSuccessStatusCode)
                    return [];

                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Response: {json}");

                var rates = JsonSerializer.Deserialize<ExchangeRateResponse>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return rates?.Rates ?? [];
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching exchange rates: {ex.Message}");
                return [];
            }
        }

        private class ExchangeRateResponse
        {
            public required Dictionary<string, decimal> Rates { get; set; }
        }
    }
}
