using Microsoft.ML;
using System.Collections.Generic;
using Utils.Models;

namespace Services.ServiceAccess
{
    public interface IFinancialForecaster
    {
        ITransformer TrainModel(IEnumerable<ForecastData> data);
        double Predict(ITransformer model, ForecastData data);
    }
}
