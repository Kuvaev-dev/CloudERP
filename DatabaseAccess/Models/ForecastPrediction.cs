using Microsoft.ML.Data;

namespace DatabaseAccess.Models
{
    public class ForecastPrediction
    {
        [ColumnName("Score")]
        public double PredictedValue { get; set; }
    }
}
