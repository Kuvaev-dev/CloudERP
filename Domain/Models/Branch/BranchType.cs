using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class BranchType
    {
        [Key]
        public int BranchTypeID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [StringLength(50, ErrorMessageResourceName = "StringLengthMaxValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public string? BranchTypeName { get; set; }
    }
}