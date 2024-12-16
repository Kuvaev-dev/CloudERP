using System;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace CloudERP.Models
{
    public class EmployeeMV
    {
        public int EmployeeID { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(15)]
        public string ContactNumber { get; set; }

        [Required]
        [StringLength(15)]
        public string TIN { get; set; }

        [Required]
        [StringLength(200)]
        public string Address { get; set; }

        [Required]
        [StringLength(100)]
        public string Designation { get; set; }

        [Required]
        [StringLength(100)]
        public string Description { get; set; }

        public double MonthlySalary { get; set; }

        public string Photo { get; set; }
        public bool IsFirstLogin { get; set; }
        public DateTime RegistrationDate { get; set; }

        [Required]
        public int CompanyID { get; set; }

        [Required]
        public int BranchID { get; set; }

        public int? UserID { get; set; }

        public HttpPostedFileBase LogoFile { get; set; }
    }
}