using System.ComponentModel.DataAnnotations;

namespace CloudERP.Models
{
    public class AccountControlMV
    {
        [Required(ErrorMessage = "Account Control ID is required.")]
        public int AccountControlID { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Company ID is required.")]
        public int CompanyID { get; set; }

        [Required(ErrorMessage = "Branch Name is required.")]
        [StringLength(100, ErrorMessage = "Branch Name cannot be longer than 100 characters.")]
        public string BranchName { get; set; }

        [Required(ErrorMessage = "Branch ID is required.")]
        public int BranchID { get; set; }

        [Required(ErrorMessage = "Account Head Name is required.")]
        [StringLength(100, ErrorMessage = "Account Head Name cannot be longer than 100 characters.")]
        public string AccountHeadName { get; set; }

        [Required(ErrorMessage = "Account Head ID is required.")]
        public int AccountHeadID { get; set; }

        [Required(ErrorMessage = "Account Control Name is required.")]
        [StringLength(100, ErrorMessage = "Account Control Name cannot be longer than 100 characters.")]
        public string AccountControlName { get; set; }

        [Required(ErrorMessage = "User Name is required.")]
        [StringLength(100, ErrorMessage = "User Name cannot be longer than 100 characters.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        public int UserID { get; set; }
    }
}