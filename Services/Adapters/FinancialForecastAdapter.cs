using Domain.Models;
using Domain.ServiceAccess;
using Microsoft.ML;

namespace Services.Adapters
{
    public class FinancialForecastAdapter : IFinancialForecastAdapter
    {
        private readonly IFinancialForecaster _financialForecaster;
        private ITransformer _trainedModel;

        public FinancialForecastAdapter(IFinancialForecaster financialForecaster)
        {
            _financialForecaster = financialForecaster;
        }

        public void Train(IEnumerable<ForecastData> data)
        {
            _trainedModel = (ITransformer)_financialForecaster.TrainModel(data);
        }

        public double Predict(ForecastData data)
        {
            return _financialForecaster.Predict(_trainedModel, data);
        }
    }
}
