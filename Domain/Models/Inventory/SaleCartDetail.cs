using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class SaleCartDetail
    {
        public int SaleCartDetailID { get; set; }

        [Required(ErrorMessage = "Product ID is required.")]
        public int ProductID { get; set; }

        [Required(ErrorMessage = "Sale Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Sale Quantity must be greater than 0.")]
        public int SaleQuantity { get; set; }

        [Required(ErrorMessage = "Sale Unit Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Sale Unit Price must be greater than 0.")]
        public double SaleUnitPrice { get; set; }

        [Required(ErrorMessage = "Company ID is required.")]
        public int CompanyID { get; set; }

        [Required(ErrorMessage = "Branch ID is required.")]
        public int BranchID { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        public int UserID { get; set; }
    }
}
