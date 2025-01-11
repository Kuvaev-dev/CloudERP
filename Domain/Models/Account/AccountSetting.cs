using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class AccountSetting
    {
        [Key]
        public int AccountSettingID { get; set; }

        [Required(ErrorMessage = "Account Head ID is required.")]
        public int AccountHeadID { get; set; }

        [Required(ErrorMessage = "Account Head Name is required.")]
        [StringLength(150, ErrorMessage = "Account Head Name cannot exceed 150 characters.")]
        public string AccountHeadName { get; set; }

        [Required(ErrorMessage = "Account Control ID is required.")]
        public int AccountControlID { get; set; }

        [Required(ErrorMessage = "Account Control Name is required.")]
        [StringLength(150, ErrorMessage = "Account Control Name cannot exceed 150 characters.")]
        public string AccountControlName { get; set; }

        [Required(ErrorMessage = "Account Sub Control ID is required.")]
        public int AccountSubControlID { get; set; }

        [Required(ErrorMessage = "Account Sub Control Name is required.")]
        [StringLength(150, ErrorMessage = "Account Sub Control Name cannot exceed 150 characters.")]
        public string AccountSubControlName { get; set; }

        [Required(ErrorMessage = "Account Activity ID is required.")]
        public int AccountActivityID { get; set; }

        [Required(ErrorMessage = "Account Activity Name is required.")]
        [StringLength(150, ErrorMessage = "Account Activity Name cannot exceed 150 characters.")]
        public string AccountActivityName { get; set; }

        [Required(ErrorMessage = "Company ID is required.")]
        public int CompanyID { get; set; }

        [Required(ErrorMessage = "Company Name is required.")]
        [StringLength(150, ErrorMessage = "Company Name cannot exceed 150 characters.")]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "Branch ID is required.")]
        public int BranchID { get; set; }

        [Required(ErrorMessage = "Branch Name is required.")]
        [StringLength(150, ErrorMessage = "Branch Name cannot exceed 150 characters.")]
        public string BranchName { get; set; }
    }
}