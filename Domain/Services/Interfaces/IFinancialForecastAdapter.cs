using Domain.Models.Forecasting;
using System.Collections.Generic;

namespace Domain.Services.Interfaces
{
    public interface IFinancialForecastAdapter
    {
        void Train(IEnumerable<ForecastData> data);
        double Predict(ForecastData data);
    }
}
