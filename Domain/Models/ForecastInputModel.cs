using Domain.Models.Forecasting;
using System;
using System.Collections.Generic;

namespace Domain.Models
{
    public class ForecastInputModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public IEnumerable<ForecastData> ForecastData { get; set; }
    }
}
