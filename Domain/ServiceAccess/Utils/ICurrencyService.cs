namespace Domain.ServiceAccess
{
    public interface ICurrencyService
    {
        Task<Dictionary<string, decimal>> GetExchangeRatesAsync(string baseCurrency);
    }
}
