using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ICurrencyService
    {
        Task<Dictionary<string, decimal>> GetExchangeRatesAsync(string baseCurrency);
    }
}
