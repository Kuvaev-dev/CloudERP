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
        public int AccountControlID { get; set; }
        public string AccountControlName { get; set; }

        [Required(ErrorMessage = "Account Head ID is required.")]
        public int AccountHeadID { get; set; }

        [StringLength(100, ErrorMessage = "Account Head Name cannot exceed 100 characters.")]
        public string AccountHeadName { get; set; }

        public int CompanyID { get; set; }
        public int BranchID { get; set; }
        public int UserID { get; set; }
        public string FullName { get; set; }
    }
}
