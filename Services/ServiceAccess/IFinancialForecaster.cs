using Domain.Models;
using Microsoft.ML;

namespace Services.ServiceAccess
{
    public interface IFinancialForecaster
    {
        ITransformer TrainModel(IEnumerable<ForecastData> data);
        double Predict(ITransformer model, ForecastData data);
    }
}
