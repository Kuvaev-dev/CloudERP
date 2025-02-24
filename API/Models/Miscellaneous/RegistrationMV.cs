namespace API.Models
{
    public class RegistrationMV
    {
        // Company details
        public string? CompanyName { get; set; }

        // Branch details
        public string? BranchName { get; set; }
        public string? BranchContact { get; set; }
        public string? BranchAddress { get; set; }

        // Employee details
        public string? EmployeeName { get; set; }
        public string? EmployeeContactNo { get; set; }
        public string? EmployeeEmail { get; set; }
        public string? EmployeeTIN { get; set; }
        public string? EmployeeDesignation { get; set; }
        public string? EmployeeDescription { get; set; }
        public float EmployeeMonthlySalary { get; set; }
        public string? EmployeeAddress { get; set; }

        // User details
        public string? UserName { get; set; }
    }
}