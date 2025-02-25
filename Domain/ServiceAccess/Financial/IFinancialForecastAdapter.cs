using Domain.Models;

namespace Domain.ServiceAccess
{
    public interface IFinancialForecastAdapter
    {
        void Train(IEnumerable<ForecastData> data);
        double Predict(ForecastData data);
    }
}
