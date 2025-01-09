using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class AccountSubControl
    {
        [Key]
        public int AccountSubControlID { get; set; }

        [Required(ErrorMessage = "Account Sub-Control Name is required.")]
        [StringLength(100, ErrorMessage = "Account Sub-Control Name cannot exceed 100 characters.")]
        public string AccountSubControlName { get; set; }

        [Required(ErrorMessage = "Account Control ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Account Control ID must be a positive integer.")]
        public int AccountControlID { get; set; }

        [StringLength(100, ErrorMessage = "Account Control Name cannot exceed 100 characters.")]
        public string AccountControlName { get; set; }

        [Required(ErrorMessage = "Account Head ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Account Head ID must be a positive integer.")]
        public int AccountHeadID { get; set; }

        [StringLength(100, ErrorMessage = "Account Head Name cannot exceed 100 characters.")]
        public string AccountHeadName { get; set; }

        [Required(ErrorMessage = "Company ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Company ID must be a positive integer.")]
        public int CompanyID { get; set; }

        [Required(ErrorMessage = "Branch ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Branch ID must be a positive integer.")]
        public int BranchID { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "User ID must be a positive integer.")]
        public int UserID { get; set; }

        [StringLength(100, ErrorMessage = "Full Name cannot exceed 100 characters.")]
        public string FullName { get; set; }
    }
}
