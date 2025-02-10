using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Utils.Interfaces;

namespace CloudERP.Helpers
{
    public class CurrencyService : ICurrencyService
    {
        private readonly string _apiUrl = ConfigurationManager.AppSettings["ExchangeRateApiUrl"];

        private readonly HttpClient _httpClient;

        public CurrencyService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Dictionary<string, decimal>> GetExchangeRatesAsync(string baseCurrency = "USD")
        {
            try
            {
                var apiUrl = $"{_apiUrl}/latest/{baseCurrency}";
                var response = await _httpClient.GetAsync(apiUrl);

                if (!response.IsSuccessStatusCode)
                    return new Dictionary<string, decimal>();

                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Response: {json}");

                var rates = JsonSerializer.Deserialize<ExchangeRateResponse>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return rates?.Rates ?? new Dictionary<string, decimal>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching exchange rates: {ex.Message}");
                return new Dictionary<string, decimal>();
            }
        }

        private class ExchangeRateResponse
        {
            public Dictionary<string, decimal> Rates { get; set; }
        }
    }
}