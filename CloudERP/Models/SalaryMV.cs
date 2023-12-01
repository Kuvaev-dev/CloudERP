using System.ComponentModel.DataAnnotations;

namespace CloudERP.Models
{
    public class SalaryMV
    {
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string CNIC { get; set; }
        public string Designation { get; set; }
        [DataType(DataType.Currency)]
        public double TransferAmount { get; set; }
        public string SalaryMonth { get; set; }
        public string SalaryYear { get; set; }
    }
}