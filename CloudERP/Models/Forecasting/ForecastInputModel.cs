using CloudERP.Models.Forecasting;
using System.Collections.Generic;
using System;

namespace CloudERP.Models
{
    public class ForecastInputModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public IEnumerable<ForecastData> ForecastData { get; set; }
    }
}