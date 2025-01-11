using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class SupplierInvoiceDetail
    {
        public int SupplierInvoiceDetailID { get; set; }

        [Required(ErrorMessage = "Supplier Invoice ID is required.")]
        public int SupplierInvoiceID { get; set; }

        [Required(ErrorMessage = "Product ID is required.")]
        public int ProductID { get; set; }

        [Required(ErrorMessage = "Product Name is required.")]
        [StringLength(150, ErrorMessage = "Product Name cannot be longer than 150 characters.")]
        public string ProductName { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Purchase Quantity must be a positive value.")]
        public int PurchaseQuantity { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Sale Quantity must be a positive value.")]
        public int SaleQuantity { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Sale Unit Price must be a positive value.")]
        public double SaleUnitPrice { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Purchase Unit Price must be a positive value.")]
        public double PurchaseUnitPrice { get; set; }

        [Required(ErrorMessage = "User Name is required.")]
        [StringLength(100, ErrorMessage = "User Name cannot be longer than 100 characters.")]
        public double UserName { get; set; }
    }
}
