using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class CustomerInvoice
    {
        [Key]
        public int CustomerInvoiceID { get; set; }

        [Required(ErrorMessage = "Customer ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Customer ID must be a positive integer.")]
        public int CustomerID { get; set; }

        [Required(ErrorMessage = "Company ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Company ID must be a positive integer.")]
        public int CompanyID { get; set; }

        [Required(ErrorMessage = "Branch ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Branch ID must be a positive integer.")]
        public int BranchID { get; set; }

        [Required(ErrorMessage = "Invoice Number is required.")]
        [StringLength(50, ErrorMessage = "Invoice Number cannot exceed 50 characters.")]
        public string InvoiceNo { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(150, ErrorMessage = "Title cannot exceed 150 characters.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Total Amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Total Amount must be greater than zero.")]
        public double TotalAmount { get; set; }

        [Required(ErrorMessage = "Invoice Date is required.")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
        public System.DateTime InvoiceDate { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "User ID must be a positive integer.")]
        public int UserID { get; set; }

        public Company Company { get; set; }
        public Branch Branch { get; set; }
        public Customer Customer { get; set; }
    }
}