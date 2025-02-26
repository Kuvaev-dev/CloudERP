using Domain.Models;

namespace Domain.ServiceAccess
{
    public interface IFinancialForecaster
    {
        object TrainModel(IEnumerable<ForecastData> data);
        double Predict(object model, ForecastData data);
    }
}
