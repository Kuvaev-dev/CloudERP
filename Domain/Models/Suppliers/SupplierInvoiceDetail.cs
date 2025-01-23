using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class SupplierInvoiceDetail
    {
        [Key]
        public int SupplierInvoiceDetailID { get; set; }

        [Required(ErrorMessage = "Supplier Invoice ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Supplier Invoice ID must be a positive integer.")]
        public int SupplierInvoiceID { get; set; }

        [Required(ErrorMessage = "Product ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Product ID must be a positive integer.")]
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
        public string UserName { get; set; }

        [Required(ErrorMessage = "Sale Cart Detail ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Sale Cart Detail ID must be a positive integer.")]
        public int SaleCartDetailID { get; set; }

        public Company Company { get; set; }
        public Supplier Supplier { get; set; }
        public SupplierInvoice SupplierInvoice { get; set; }
        public Branch Branch { get; set; }
        public SupplierReturnInvoiceDetail SupplierReturnInvoiceDetail { get; set; }
        public Stock Stock { get; set; }
    }
}
