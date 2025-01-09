namespace Domain.Models
{
    public class Payroll
    {
        public int PayrollID { get; set; }
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public int BranchID { get; set; }
        public string BranchName { get; set; }
        public string BranchAddress { get; set; }
        public int CompanyID { get; set; }
        public string CompanyName { get; set; }
        public string CompanyLogo { get; set; }
        public double TransferAmount { get; set; }
        public string PayrollInvoiceNo { get; set; }
        public System.DateTime PaymentDate { get; set; }
        public string SalaryMonth { get; set; }
        public string SalaryYear { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }
    }
}
