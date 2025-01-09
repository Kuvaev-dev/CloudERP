using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeID { get; set; }

        [Required(ErrorMessage = "Full Name is required.")]
        [StringLength(100, ErrorMessage = "Full Name cannot exceed 100 characters.")]
        public string FullName { get; set; }

        [StringLength(15, ErrorMessage = "Contact Number cannot exceed 15 characters.")]
        [Phone(ErrorMessage = "Contact Number must be a valid phone number.")]
        public string ContactNumber { get; set; }

        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
        public string Email { get; set; }

        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        public string Address { get; set; }

        public string Photo { get; set; }

        [StringLength(20, ErrorMessage = "TIN cannot exceed 20 characters.")]
        public string TIN { get; set; }

        [StringLength(50, ErrorMessage = "Designation cannot exceed 50 characters.")]
        public string Designation { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Monthly Salary must be a positive value.")]
        public double MonthlySalary { get; set; }

        public bool? IsFirstLogin { get; set; }

        public DateTime? RegistrationDate { get; set; }

        [Required(ErrorMessage = "Company ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Company ID must be a positive integer.")]
        public int CompanyID { get; set; }

        [Required(ErrorMessage = "Branch ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Branch ID must be a positive integer.")]
        public int BranchID { get; set; }

        [Required(ErrorMessage = "Branch Type ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Branch Type ID must be a positive integer.")]
        public int BranchTypeID { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Branch Code (BrchID) must be a positive integer.")]
        public int? BrchID { get; set; }

        [StringLength(100, ErrorMessage = "Branch Name cannot exceed 100 characters.")]
        public string BranchName { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "User ID must be a positive integer.")]
        public int? UserID { get; set; }
    }
}
