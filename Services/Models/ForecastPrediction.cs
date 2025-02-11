using Microsoft.ML.Data;

namespace Services.Models
{
    public class ForecastPrediction
    {
        [ColumnName("Score")]
        public double PredictedValue { get; set; }
    }
}