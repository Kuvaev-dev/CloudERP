using System;
using System.ComponentModel.DataAnnotations;

namespace CloudERP.Models
{
    public class StockMV
    {
        public int ProductID { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        public int CategoryID { get; set; }

        [Required(ErrorMessage = "Product name is required.")]
        [StringLength(100, ErrorMessage = "Product name cannot exceed 100 characters.")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a positive value.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Sale unit price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Sale unit price must be a positive value.")]
        public double SaleUnitPrice { get; set; }

        [Required(ErrorMessage = "Current purchase unit price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Purchase unit price must be a positive value.")]
        public double CurrentPurchaseUnitPrice { get; set; }

        [Required(ErrorMessage = "Expiry date is required.")]
        [DataType(DataType.Date)]
        public DateTime ExpiryDate { get; set; }

        [Required(ErrorMessage = "Manufacture date is required.")]
        [DataType(DataType.Date)]
        public DateTime Manufacture { get; set; }

        [Required(ErrorMessage = "Stock threshold quantity is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock threshold quantity must be a positive value.")]
        public int StockTreshHoldQuantity { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        public int UserID { get; set; }
        public int BranchID { get; set; }
        public int CompanyID { get; set; }

        public bool? IsActive { get; set; }
    }
}