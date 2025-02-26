using Domain.Models;
using Domain.ServiceAccess;
using Microsoft.ML;
using Services.Models;

namespace Services.Implementations
{
    public class FinancialForecaster : IFinancialForecaster
    {
        private readonly MLContext _mlContext;

        public FinancialForecaster(MLContext mlContext)
        {
            _mlContext = mlContext;
        }

        public object TrainModel(IEnumerable<ForecastData> data)
        {
            var dataView = _mlContext.Data.LoadFromEnumerable(data);

            var pipeline = _mlContext.Transforms.Concatenate("Features", "DateAsNumber")
                .Append(_mlContext.Regression.Trainers.Sdca(labelColumnName: "Value", maximumNumberOfIterations: 100));

            return pipeline.Fit(dataView);
        }

        public double Predict(object model, ForecastData data)
        {
            var transformer = model as ITransformer;
            var predictionEngine = _mlContext.Model.CreatePredictionEngine<ForecastData, ForecastPrediction>(transformer);
            var prediction = predictionEngine.Predict(data);
            return prediction.PredictedValue;
        }
    }
}