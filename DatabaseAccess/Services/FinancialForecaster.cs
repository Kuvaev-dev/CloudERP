using DatabaseAccess.Models;
using Domain.Models.Forecasting;
using Microsoft.ML;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseAccess.Services
{
    public interface IFinancialForecaster
    {
        ITransformer TrainModel(IEnumerable<ForecastData> data);
        double Predict(ITransformer model, ForecastData data);
    }

    public class FinancialForecaster : IFinancialForecaster
    {
        private readonly MLContext _mlContext;

        public FinancialForecaster(MLContext mlContext)
        {
            _mlContext = mlContext;
        }

        public ITransformer TrainModel(IEnumerable<ForecastData> data)
        {
            var dataView = _mlContext.Data.LoadFromEnumerable(data);

            var pipeline = _mlContext.Transforms.Concatenate("Features", "DateAsNumber")
                .Append(_mlContext.Regression.Trainers.Sdca(labelColumnName: "Value", maximumNumberOfIterations: 100));

            return pipeline.Fit(dataView);
        }

        public double Predict(ITransformer model, ForecastData data)
        {
            var predictionEngine = _mlContext.Model.CreatePredictionEngine<ForecastData, ForecastPrediction>(model);
            var prediction = predictionEngine.Predict(data);
            return prediction.PredictedValue;
        }
    }
}