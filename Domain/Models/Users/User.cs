using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [StringLength(100, ErrorMessageResourceName = "StringLengthMaxValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public string? FullName { get; set; }

        [Required(ErrorMessageResourceName = "RequiredValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [EmailAddress(ErrorMessageResourceName = "EmailValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [StringLength(100, ErrorMessageResourceName = "StringLengthMaxValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public string? Email { get; set; }

        [Required(ErrorMessageResourceName = "RequiredValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [Phone(ErrorMessageResourceName = "PhoneNumberValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [StringLength(15, ErrorMessageResourceName = "StringLengthMaxValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public string? ContactNo { get; set; }

        [Required(ErrorMessageResourceName = "RequiredValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [StringLength(50, ErrorMessageResourceName = "StringLengthMaxValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public string? UserName { get; set; }

        [Required(ErrorMessageResourceName = "RequiredValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [StringLength(255, MinimumLength = 6, ErrorMessageResourceName = "StringLengthMinMaxValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public string? Password { get; set; }
        public string? Salt { get; set; }
        public int UserTypeID { get; set; }
        public bool IsActive { get; set; }
        public string? UserTypeName { get; set; }
        public string? BranchName { get; set; }
        public string? ResetPasswordCode { get; set; }
        public DateTime? LastPasswordResetRequest { get; set; }
        public DateTime? ResetPasswordExpiration { get; set; }
    }
}