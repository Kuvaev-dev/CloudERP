using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [StringLength(100, ErrorMessageResourceName = "StringLengthMaxValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public string? FullName { get; set; }

        [Required(ErrorMessageResourceName = "RequiredValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [StringLength(15, ErrorMessageResourceName = "StringLengthMaxValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [Phone(ErrorMessageResourceName = "PhoneValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public string? ContactNumber { get; set; }

        [Required(ErrorMessageResourceName = "RequiredValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [EmailAddress(ErrorMessageResourceName = "EmailValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [StringLength(100, ErrorMessageResourceName = "StringLengthMaxValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public string? Email { get; set; }

        [Required(ErrorMessageResourceName = "RequiredValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [StringLength(200, ErrorMessageResourceName = "StringLengthMaxValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public string? Address { get; set; }

        public string? Photo { get; set; }

        [Required(ErrorMessageResourceName = "RequiredValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [StringLength(20, ErrorMessageResourceName = "StringLengthMaxValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public string? TIN { get; set; }

        [Required(ErrorMessageResourceName = "RequiredValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [StringLength(50, ErrorMessageResourceName = "StringLengthMaxValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public string? Designation { get; set; }

        [Required(ErrorMessageResourceName = "RequiredValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [StringLength(500, ErrorMessageResourceName = "StringLengthMaxValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public string? Description { get; set; }

        [Required(ErrorMessageResourceName = "RequiredValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [Range(1, double.MaxValue, ErrorMessageResourceName = "RangeValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public double MonthlySalary { get; set; }

        public bool? IsFirstLogin { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? RegistrationDate { get; set; }
        public int CompanyID { get; set; }
        public int BranchID { get; set; }
        public int BranchTypeID { get; set; }
        public int? BrchID { get; set; }
        public string? BranchName { get; set; }
        public int? UserID { get; set; }
    }
}