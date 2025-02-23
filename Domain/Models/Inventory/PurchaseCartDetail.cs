using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class PurchaseCartDetail
    {
        public int PurchaseCartDetailID { get; set; }

        [Required(ErrorMessage = "Product ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Product ID must be a positive integer.")]
        public int ProductID { get; set; }

        [Required(ErrorMessage = "Product Name is required.")]
        public string? ProductName { get; set; }

        [Required(ErrorMessage = "Purchase Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Purchase Quantity must be greater than 0.")]
        public int PurchaseQuantity { get; set; }

        [Required(ErrorMessage = "Purchase Unit Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Purchase Unit Price must be greater than 0.")]
        public double PurchaseUnitPrice { get; set; }

        [Required(ErrorMessage = "Company ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Company ID must be a positive integer.")]
        public int CompanyID { get; set; }

        [Required(ErrorMessage = "Branch ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Branch ID must be a positive integer.")]
        public int BranchID { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "User ID must be a positive integer.")]
        public int UserID { get; set; }

        [Required(ErrorMessage = "User Name is required.")]
        public string? UserName { get; set; }
    }
}