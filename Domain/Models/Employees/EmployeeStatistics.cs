using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class EmployeeStatistics
    {
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        public int NumberOfRegistrations { get; set; }
    }
}
