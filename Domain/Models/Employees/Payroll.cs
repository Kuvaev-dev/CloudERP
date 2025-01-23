using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class Payroll
    {
        [Key]
        public int PayrollID { get; set; }

        [Required(ErrorMessage = "Employee ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Employee ID must be a positive integer.")]
        public int EmployeeID { get; set; }

        [Required(ErrorMessage = "Employee Name is required.")]
        [StringLength(100, ErrorMessage = "Employee Name cannot exceed 100 characters.")]
        public string EmployeeName { get; set; }

        [Required(ErrorMessage = "Branch ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Branch ID must be a positive integer.")]
        public int BranchID { get; set; }

        [Required(ErrorMessage = "Branch Name is required.")]
        [StringLength(100, ErrorMessage = "Branch Name cannot exceed 100 characters.")]
        public string BranchName { get; set; }

        [Required(ErrorMessage = "Branch Address is required.")]
        [StringLength(200, ErrorMessage = "Branch Address cannot exceed 200 characters.")]
        public string BranchAddress { get; set; }

        [Required(ErrorMessage = "Branch Contact is required.")]
        [StringLength(200, ErrorMessage = "Branch Contact cannot exceed 200 characters.")]
        public string BranchContact { get; set; }

        [Required(ErrorMessage = "Company ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Company ID must be a positive integer.")]
        public int CompanyID { get; set; }

        [Required(ErrorMessage = "Company Name is required.")]
        [StringLength(100, ErrorMessage = "Company Name cannot exceed 100 characters.")]
        public string CompanyName { get; set; }

        [StringLength(200, ErrorMessage = "Company Logo path cannot exceed 200 characters.")]
        public string CompanyLogo { get; set; }

        [Required(ErrorMessage = "Transfer Amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Transfer Amount must be greater than zero.")]
        public double TransferAmount { get; set; }

        [Required(ErrorMessage = "Payroll Invoice Number is required.")]
        [StringLength(50, ErrorMessage = "Payroll Invoice Number cannot exceed 50 characters.")]
        public string PayrollInvoiceNo { get; set; }

        [Required(ErrorMessage = "Payment Date is required.")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
        public DateTime PaymentDate { get; set; }

        [Required(ErrorMessage = "Salary Month is required.")]
        [StringLength(20, ErrorMessage = "Salary Month cannot exceed 20 characters.")]
        public string SalaryMonth { get; set; }

        [Required(ErrorMessage = "Salary Year is required.")]
        [StringLength(4, ErrorMessage = "Salary Year must be 4 characters.")]
        public string SalaryYear { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "User ID must be a positive integer.")]
        public int UserID { get; set; }

        [Required(ErrorMessage = "User Name is required.")]
        [StringLength(100, ErrorMessage = "User Name cannot exceed 100 characters.")]
        public string UserName { get; set; }
    }
}