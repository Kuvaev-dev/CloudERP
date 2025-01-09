using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class AccountHead
    {
        [Key]
        public int AccountHeadID { get; set; }

        [Required(ErrorMessage = "Account head name is required.")]
        [StringLength(100, ErrorMessage = "Account head name cannot exceed 100 characters.")]
        public string AccountHeadName { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Code must be a positive number.")]
        public int Code { get; set; }

        [Required(ErrorMessage = "UserID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "UserID must be a positive integer.")]
        public int UserID { get; set; }

        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters.")]
        public string FullName { get; set; }
    }
}