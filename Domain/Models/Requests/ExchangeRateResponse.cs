namespace Domain.Models
{
    public class ExchangeRateResponse
    {
        public required Dictionary<string, decimal> Rates { get; set; }
    }
}
