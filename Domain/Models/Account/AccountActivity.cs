using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class AccountActivity
    {
        [Key]
        public int AccountActivityID { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Company ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Company ID must be a positive integer.")]
        public int CompanyID { get; set; }

        [Required(ErrorMessage = "Company Name is required.")]
        [StringLength(150, ErrorMessage = "Company Name cannot exceed 150 characters.")]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "Branch ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Branch ID must be a positive integer.")]
        public int BranchID { get; set; }

        [Required(ErrorMessage = "Branch Name is required.")]
        [StringLength(150, ErrorMessage = "Branch Name cannot exceed 150 characters.")]
        public string BranchName { get; set; }
    }
}
