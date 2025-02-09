using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class AccountSetting
    {
        [Key]
        public int AccountSettingID { get; set; }

        [Required(ErrorMessage = "Account Head ID is required.")]
        public int AccountHeadID { get; set; }
        public string AccountHeadName { get; set; }

        [Required(ErrorMessage = "Account Control ID is required.")]
        public int AccountControlID { get; set; }
        public string AccountControlName { get; set; }

        [Required(ErrorMessage = "Account Sub Control ID is required.")]
        public int AccountSubControlID { get; set; }
        public string AccountSubControlName { get; set; }

        [Required(ErrorMessage = "Account Activity ID is required.")]
        public int AccountActivityID { get; set; }
        public string AccountActivityName { get; set; }

        public int CompanyID { get; set; }
        public string CompanyName { get; set; }
        public int BranchID { get; set; }
        public string BranchName { get; set; }
        public int UserID { get; set; }
        public string FullName { get; set; }
        public bool IsGlobal { get; set; }
    }
}