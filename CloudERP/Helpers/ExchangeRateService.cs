using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace CloudERP.Helpers
{
    public class ExchangeRateService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _defaultCurrency = System.Configuration.ConfigurationManager.AppSettings["DefaultCurrency"];

        public ExchangeRateService(string apiKey)
        {
            _httpClient = new HttpClient();
            _apiKey = apiKey;
        }

        public async Task<Dictionary<string, double>> GetExchangeRatesAsync()
        {
            var response = await _httpClient.GetStringAsync($"https://api.exchangerate-api.com/v4/latest/{_defaultCurrency}?apikey={_apiKey}");
            var exchangeRates = JsonConvert.DeserializeObject<ExchangeRateResponse>(response);
            return exchangeRates.Rates;
        }

        private class ExchangeRateResponse
        {
            public Dictionary<string, double> Rates { get; set; }
        }
    }
}