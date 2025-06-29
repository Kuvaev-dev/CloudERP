﻿namespace API.Models
{
    public class SalaryMV
    {
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string TIN { get; set; }
        public string Designation { get; set; }
        public double TransferAmount { get; set; }
        public string SalaryMonth { get; set; }
        public string SalaryYear { get; set; }
        public double? BonusPercentage { get; set; }
        public double TotalAmount { get; set; }
        public int CompanyID { get; set; }
        public int BranchID { get; set; }
        public int UserID { get; set; }
    }
}