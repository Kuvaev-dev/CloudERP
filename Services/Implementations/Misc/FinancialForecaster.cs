using Domain.Models;
using Microsoft.ML;
using Services.Models;
using Services.ServiceAccess;

namespace Services.Implementations
{
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