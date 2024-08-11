using CloudERP.Models.Forecasting;
using Microsoft.ML;
using System.Collections.Generic;
using System.Linq;

namespace CloudERP.Helpers
{
    public class FinancialForecaster
    {
        private readonly MLContext _mlContext;

        public FinancialForecaster()
        {
            _mlContext = new MLContext();
        }

        public ITransformer TrainModel(IEnumerable<ForecastData> data)
        {
            var dataView = _mlContext.Data.LoadFromEnumerable(data);
            var pipeline = _mlContext.Transforms.Concatenate("Features", "Date")
                .Append(_mlContext.Regression.Trainers.Sdca(labelColumnName: "Value", maximumNumberOfIterations: 100));
            return pipeline.Fit(dataView);
        }

        public float Predict(ITransformer model, ForecastData data)
        {
            var predictionEngine = _mlContext.Model.CreatePredictionEngine<ForecastData, ForecastPrediction>(model);
            var prediction = predictionEngine.Predict(data);
            return prediction.PredictedValue;
        }
    }
}