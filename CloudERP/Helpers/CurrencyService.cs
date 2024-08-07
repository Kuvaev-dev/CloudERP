using System.Collections.Generic;
using System.Threading.Tasks;

namespace CloudERP.Helpers
{
    public class CurrencyService : ICurrencyService
    {
        private readonly ExchangeRateService _exchangeRateService;
        private readonly string _defaultCurrency;

        public CurrencyService(string exchangeRateApiKey, string defaultCurrency)
        {
            _exchangeRateService = new ExchangeRateService(exchangeRateApiKey);
            _defaultCurrency = defaultCurrency;
        }

        public async Task<Dictionary<string, double>> GetExchangeRatesAsync()
        {
            return await _exchangeRateService.GetExchangeRatesAsync() ?? new Dictionary<string, double>();
        }

        public async Task<string> GetSelectedCurrencyAsync(string sessionCurrency, string defaultCurrency)
        {
            return sessionCurrency ?? defaultCurrency;
        }

        public async Task<double> GetConversionRateAsync(Dictionary<string, double> rates, string fromCurrency, string toCurrency)
        {
            if (rates.TryGetValue(fromCurrency, out double fromRate) && rates.TryGetValue(toCurrency, out double toRate))
            {
                return fromRate / toRate;
            }

            return 1.0;
        }
    }
}