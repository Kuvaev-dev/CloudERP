using Microsoft.ML.Data;

namespace CloudERP.Models.Forecasting
{
    public class ForecastPrediction
    {
        [ColumnName("Score")]
        public float PredictedValue { get; set; }
    }
}