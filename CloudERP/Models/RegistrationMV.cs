using System.ComponentModel.DataAnnotations;

namespace CloudERP.Models
{
    public class RegistrationMV
    {
        // Company details
        [Required(ErrorMessage = "Company Name is required")]
        public string CompanyName { get; set; }

        // Branch details
        [Required(ErrorMessage = "Branch Name is required")]
        public string BranchName { get; set; }

        [Required(ErrorMessage = "Branch Contact Number is required")]
        public string BranchContact { get; set; }

        [Required(ErrorMessage = "Branch Address is required")]
        public string BranchAddress { get; set; }

        // Employee details
        [Required(ErrorMessage = "Employee Name is required")]
        public string EmployeeName { get; set; }

        [Required(ErrorMessage = "Employee Contact Number is required")]
        public string EmployeeContactNo { get; set; }

        [Required(ErrorMessage = "Employee Email Address is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string EmployeeEmail { get; set; }

        [Required(ErrorMessage = "Employee TIN is required")]
        public string EmployeeTIN { get; set; }

        [Required(ErrorMessage = "Employee Designation is required")]
        public string EmployeeDesignation { get; set; }

        [Required(ErrorMessage = "Employee Monthly Salary is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Monthly Salary must be greater than 0")]
        public float EmployeeMonthlySalary { get; set; }

        [Required(ErrorMessage = "Employee Address is required")]
        public string EmployeeAddress { get; set; }

        // User details
        [Required(ErrorMessage = "Username is required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}