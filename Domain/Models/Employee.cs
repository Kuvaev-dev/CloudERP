using System;

namespace Domain.Models
{
    public class Employee
    {
        public int EmployeeID { get; set; }
        public string FullName { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Photo { get; set; }
        public string TIN { get; set; }
        public string Designation { get; set; }
        public string Description { get; set; }
        public double MonthlySalary { get; set; }
        public bool IsFirstLogin { get; set; }
        public DateTime RegistrationDate { get; set; }
        public int CompanyID { get; set; }
        public int BranchID { get; set; }
        public string BranchName { get; set; }
        public int? UserID { get; set; }
    }
}
