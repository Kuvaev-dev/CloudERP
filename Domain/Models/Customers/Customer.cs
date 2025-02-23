using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class Customer
    {
        [Key]
        public int CustomerID { get; set; }

        [Required(ErrorMessage = "Customer Name is required.")]
        [StringLength(150, ErrorMessage = "Customer Name cannot exceed 150 characters.")]
        public string? Customername { get; set; }

        [Required(ErrorMessage = "Customer Contact is required.")]
        [StringLength(15, ErrorMessage = "Customer Contact cannot exceed 15 characters.")]
        [RegularExpression(@"^\+?\d{1,4}?[\d\s\(\)-]{7,15}$", ErrorMessage = "Invalid phone number format.")]
        public string? CustomerContact { get; set; }

        [StringLength(250, ErrorMessage = "Customer Address cannot exceed 250 characters.")]
        public string? CustomerArea { get; set; }

        [StringLength(250, ErrorMessage = "Customer Address cannot exceed 250 characters.")]
        public string? CustomerAddress { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }
        public int BranchID { get; set; }
        public string? BranchName { get; set; }
        public int CompanyID { get; set; }
        public string? CompanyName { get; set; }
        public int UserID { get; set; }
        public string? UserName { get; set; }
    }
}
