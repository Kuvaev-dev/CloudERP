using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class AccountControl
    {
        [Key]
        public int AccountControlID { get; set; }

        [Required(ErrorMessage = "Account Control Name is required.")]
        [StringLength(100, ErrorMessage = "AccountControlName must not exceed 100 characters.")]
        public string? AccountControlName { get; set; }

        [Required(ErrorMessage = "AccountHead ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Account Head ID must be a positive integer.")]
        public int AccountHeadID { get; set; }
        public string? AccountHeadName { get; set; }
        public int BranchID { get; set; }
        public int CompanyID { get; set; }
        public int UserID { get; set; }
        public string? FullName { get; set; }
        public bool? IsGlobal { get; set; }
    }
}
