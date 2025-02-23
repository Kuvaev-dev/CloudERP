namespace Domain.Models.FinancialModels
{
    public class Salary
    {
        public int EmployeeID { get; set; }
        public string? SalaryMonth { get; set; }
        public string? SalaryYear { get; set; }
        public double TransferAmount { get; set; }
    }
}
