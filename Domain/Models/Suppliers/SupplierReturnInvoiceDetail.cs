using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class SupplierReturnInvoiceDetail
    {
        [Key]
        public int SupplierReturnInvoiceDetailID { get; set; }

        [Required(ErrorMessage = "Supplier Invoice ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Supplier Invoice ID must be a positive integer.")]
        public int SupplierInvoiceID { get; set; }

        [Required(ErrorMessage = "Supplier Invoice Detail ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Supplier Invoice Detail ID must be a positive integer.")]
        public int SupplierInvoiceDetailID { get; set; }

        [Required(ErrorMessage = "Supplier Return Invoice ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Supplier Return Invoice ID must be a positive integer.")]
        public int SupplierReturnInvoiceID { get; set; }

        [Required(ErrorMessage = "Product ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Product ID must be a positive integer.")]
        public int ProductID { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Purchase Return Quantity must be greater than zero.")]
        public int PurchaseReturnQuantity { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Purchase Return Unit Price must be a positive value.")]
        public double PurchaseReturnUnitPrice { get; set; }
    }
}
