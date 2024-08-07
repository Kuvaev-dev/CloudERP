using System.Collections.Generic;
using System.Threading.Tasks;

public interface ICurrencyService
{
    Task<Dictionary<string, double>> GetExchangeRatesAsync();
    Task<string> GetSelectedCurrencyAsync(string sessionCurrency, string defaultCurrency);
    Task<double> GetConversionRateAsync(Dictionary<string, double> rates, string fromCurrency, string toCurrency);
}