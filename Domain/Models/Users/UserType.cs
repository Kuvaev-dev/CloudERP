using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class UserType
    {
        [Key]
        public int UserTypeID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [StringLength(50, ErrorMessageResourceName = "StringLengthMaxValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public string? UserTypeName { get; set; }
    }
}