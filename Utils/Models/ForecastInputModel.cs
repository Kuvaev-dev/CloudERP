using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Utils.Models
{
    public class ForecastInputModel
    {
        [Required(ErrorMessage = "Start Date is required.")]
        [DataType(DataType.Date, ErrorMessage = "Invalid Start Date format.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End Date is required.")]
        [DataType(DataType.Date, ErrorMessage = "Invalid End Date format.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Compare("StartDate", ErrorMessage = "End Date must be greater than or equal to Start Date.")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Forecast Data is required.")]
        [MinLength(1, ErrorMessage = "At least one forecast data entry is required.")]
        public IEnumerable<ForecastData> ForecastData { get; set; }
    }
}