using Microsoft.ML.Data;

namespace DatabaseAccess.Services
{
    public class ForecastPrediction
    {
        [ColumnName("Score")]
        public double PredictedValue { get; set; }
    }
}
