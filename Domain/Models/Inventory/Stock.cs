using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class Stock
    {
        [Key]
        public int ProductID { get; set; }
        public int CategoryID { get; set; }
        public string? CategoryName { get; set; }

        [Required(ErrorMessage = "Product Name is required.")]
        [StringLength(200, ErrorMessage = "Product Name cannot be longer than 200 characters.")]
        public string? ProductName { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a non-negative value.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Sale Unit Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Sale Unit Price must be greater than 0.")]
        public double SaleUnitPrice { get; set; }

        [Required(ErrorMessage = "Current Purchase Unit Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Current Purchase Unit Price must be greater than 0.")]
        public double CurrentPurchaseUnitPrice { get; set; }

        [Required(ErrorMessage = "Expiry Date is required.")]
        [DataType(DataType.Date)]
        public DateTime ExpiryDate { get; set; }

        [Required(ErrorMessage = "Manufacture Date is required.")]
        [DataType(DataType.Date)]
        public DateTime Manufacture { get; set; }

        [Required(ErrorMessage = "Stock Threshold Quantity is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock Threshold Quantity must be a non-negative value.")]
        public int StockTreshHoldQuantity { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters.")]
        public string? Description { get; set; }
        public int UserID { get; set; }
        public string? UserName { get; set; }
        public int BranchID { get; set; }
        public string? BranchName { get; set; }
        public int CompanyID { get; set; }
        public string? CompanyName { get; set; }
        public bool? IsActive { get; set; }
    }
}