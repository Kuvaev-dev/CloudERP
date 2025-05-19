using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class Branch
    {
        [Key]
        public int BranchID { get; set; }
        public int BrchID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [StringLength(100, ErrorMessageResourceName = "StringLengthMaxValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public string? BranchName { get; set; }

        [Required(ErrorMessageResourceName = "RequiredValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [StringLength(50, ErrorMessageResourceName = "StringLengthMaxValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [Phone(ErrorMessageResourceName = "PhoneValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public string? BranchContact { get; set; }

        [Required(ErrorMessageResourceName = "RequiredValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [StringLength(500, ErrorMessageResourceName = "StringLengthMaxValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public string? BranchAddress { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        [Required(ErrorMessageResourceName = "RequiredValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public int CompanyID { get; set; }
        public int? ParentBranchID { get; set; }
        public int BranchTypeID { get; set; }
        public string? BranchTypeName { get; set; }
    }
}