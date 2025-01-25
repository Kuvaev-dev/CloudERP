using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class SaleCartDetail
    {
        [Key]
        public int SaleCartDetailID { get; set; }

        [Required(ErrorMessage = "Product ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Product ID must be a positive integer.")]
        public int ProductID { get; set; }
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Sale Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Sale Quantity must be greater than 0.")]
        public int SaleQuantity { get; set; }

        [Required(ErrorMessage = "Sale Unit Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Sale Unit Price must be greater than 0.")]
        public double SaleUnitPrice { get; set; }
        public int CompanyID { get; set; }
        public int BranchID { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }
    }
}
