using System.ComponentModel.DataAnnotations;

namespace CloudERP.Models
{
    public class AccountHeadMV
    {
        public int AccountHeadID { get; set; }

        [Required(ErrorMessage = "Account Head Name is required.")]
        [StringLength(100, ErrorMessage = "Account Head Name must not exceed 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Only letters and spaces are allowed.")]
        public string AccountHeadName { get; set; }

        [Required(ErrorMessage = "Code is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Code must be a positive number.")]
        public int Code { get; set; }

        public int UserID { get; set; }
    }
}