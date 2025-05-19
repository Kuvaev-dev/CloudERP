using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class AccountSetting
    {
        [Key]
        public int AccountSettingID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public int AccountHeadID { get; set; }
        public string? AccountHeadName { get; set; }

        [Required(ErrorMessageResourceName = "RequiredValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public int AccountControlID { get; set; }
        public string? AccountControlName { get; set; }

        [Required(ErrorMessageResourceName = "RequiredValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public int AccountSubControlID { get; set; }
        public string? AccountSubControlName { get; set; }

        [Required(ErrorMessageResourceName = "RequiredValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public int AccountActivityID { get; set; }
        public string? AccountActivityName { get; set; }

        public int CompanyID { get; set; }
        public string? CompanyName { get; set; }
        public int BranchID { get; set; }
        public string? BranchName { get; set; }
        public int UserID { get; set; }
        public string? FullName { get; set; }
        public bool IsGlobal { get; set; }
    }
}