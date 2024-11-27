using System.ComponentModel.DataAnnotations;

namespace CloudERP.Models
{
    public class AccountControlMV
    {
        [Required(ErrorMessage = "Account ID is required.")]
        public int AccountControlID { get; set; }

        [Required(ErrorMessage = "Account name is required.")]
        [StringLength(100, ErrorMessage = "Account name must not exceed 100 characters.")]
        public string AccountControlName { get; set; }

        [Required(ErrorMessage = "Head account ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Head account ID must be greater than zero.")]
        public int AccountHeadID { get; set; }

        [Required(ErrorMessage = "Branch ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Branch ID must be greater than zero.")]
        public int BranchID { get; set; }

        [Required(ErrorMessage = "Company ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Company ID must be greater than zero.")]
        public int CompanyID { get; set; }

        [Required(ErrorMessage = "User  ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "User  ID must be greater than zero.")]
        public int UserID { get; set; }
    }
}