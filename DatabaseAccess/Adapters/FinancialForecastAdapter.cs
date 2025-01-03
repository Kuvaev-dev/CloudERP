using DatabaseAccess.Services;
using Domain.Interfaces;
using Domain.Models.Forecasting;
using Microsoft.ML;
using System.Collections.Generic;

namespace DatabaseAccess.Adapters
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
            _trainedModel = _financialForecaster.TrainModel(data);
        }

        public double Predict(ForecastData data)
        {
            return _financialForecaster.Predict(_trainedModel, data);
        }
    }
}
