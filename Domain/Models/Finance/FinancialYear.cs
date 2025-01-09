using System;

namespace Domain.Models
{
    public class FinancialYear
    {
        public int FinancialYearID { get; set; }
        public string FinancialYearName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public int UserID { get; set; }
    }
}
