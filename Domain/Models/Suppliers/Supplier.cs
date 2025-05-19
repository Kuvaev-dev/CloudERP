using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class Supplier
    {
        [Key]
        public int SupplierID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [StringLength(100, ErrorMessageResourceName = "StringLengthMaxValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public string? SupplierName { get; set; }

        [Required(ErrorMessageResourceName = "RequiredValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [StringLength(250, ErrorMessageResourceName = "StringLengthMaxValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public string? SupplierAddress { get; set; }

        [Required(ErrorMessageResourceName = "RequiredValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [StringLength(50, ErrorMessageResourceName = "StringLengthMaxValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [Phone(ErrorMessageResourceName = "PhoneValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public string? SupplierConatctNo { get; set; }

        [Required(ErrorMessageResourceName = "RequiredValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [EmailAddress(ErrorMessageResourceName = "EmailValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public string? SupplierEmail { get; set; }

        [StringLength(500, ErrorMessageResourceName = "StringLengthMaxValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public string? Discription { get; set; }
        public int CompanyID { get; set; }
        public string? CompanyName { get; set; }
        public int BranchID { get; set; }
        public string? BranchName { get; set; }
        public int UserID { get; set; }
        public string? UserName { get; set; }
        public bool? IsActive { get; set; }
    }
}