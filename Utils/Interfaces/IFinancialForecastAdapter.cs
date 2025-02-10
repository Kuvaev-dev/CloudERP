using System.Collections.Generic;
using Utils.Models;

namespace Utils.Interfaces
{
    public interface IFinancialForecastAdapter
    {
        void Train(IEnumerable<ForecastData> data);
        double Predict(ForecastData data);
    }
}
