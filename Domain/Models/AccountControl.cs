using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class AccountControl
    {
        [Key]
        public int AccountControlID { get; set; }

        [Required(ErrorMessage = "AccountControlName is required.")]
        [StringLength(100, ErrorMessage = "AccountControlName must not exceed 100 characters.")]
        public string AccountControlName { get; set; }

        [Required(ErrorMessage = "AccountHeadID is required.")]
        public int AccountHeadID { get; set; }

        [StringLength(100, ErrorMessage = "AccountHeadName must not exceed 100 characters.")]
        public string AccountHeadName { get; set; }

        [Required(ErrorMessage = "BranchID is required.")]
        public int BranchID { get; set; }

        [Required(ErrorMessage = "CompanyID is required.")]
        public int CompanyID { get; set; }

        [Required(ErrorMessage = "UserID is required.")]
        public int UserID { get; set; }

        [StringLength(200, ErrorMessage = "FullName must not exceed 200 characters.")]
        public string FullName { get; set; }
    }
}
