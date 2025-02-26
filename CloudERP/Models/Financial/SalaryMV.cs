namespace CloudERP.Models
{
    public class SalaryMV
    {
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string TIN { get; set; }
        public string Designation { get; set; }
        public decimal TransferAmount { get; set; }
        public string SalaryMonth { get; set; }
        public string SalaryYear { get; set; }
        public decimal? BonusPercentage { get; set; }
        public decimal TotalAmount { get; set; }
    }
}