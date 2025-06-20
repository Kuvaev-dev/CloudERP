using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class AccountHead
    {
        [Key]
        public int AccountHeadID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [StringLength(100, ErrorMessageResourceName = "StringLengthMaxValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public string? AccountHeadName { get; set; }

        [Range(1, int.MaxValue, ErrorMessageResourceName = "RangeValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public int Code { get; set; }
        public int UserID { get; set; }
        public string? FullName { get; set; }
    }
}