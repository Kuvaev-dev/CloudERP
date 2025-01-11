using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class Customer
    {
        [Key]
        public int CustomerID { get; set; }

        [Required(ErrorMessage = "Customer Name is required.")]
        [StringLength(150, ErrorMessage = "Customer Name cannot exceed 150 characters.")]
        public string Customername { get; set; }

        [Required(ErrorMessage = "Customer Contact is required.")]
        [StringLength(15, ErrorMessage = "Customer Contact cannot exceed 15 characters.")]
        [RegularExpression(@"^\+?\d{1,4}?[\d\s\(\)-]{7,15}$", ErrorMessage = "Invalid phone number format.")]
        public string CustomerContact { get; set; }

        [StringLength(250, ErrorMessage = "Customer Address cannot exceed 250 characters.")]
        public string CustomerAddress { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Branch ID is required.")]
        public int BranchID { get; set; }

        [Required(ErrorMessage = "Company ID is required.")]
        public int CompanyID { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        public int UserID { get; set; }

        [StringLength(150, ErrorMessage = "Branch Name cannot exceed 150 characters.")]
        public string BranchName { get; set; }

        [StringLength(150, ErrorMessage = "Company Name cannot exceed 150 characters.")]
        public string CompanyName { get; set; }

        [StringLength(150, ErrorMessage = "User Name cannot exceed 150 characters.")]
        public string UserName { get; set; }
    }
}
