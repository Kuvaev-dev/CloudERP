using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models.FinancialModels
{
    public class PurchaseReturnConfirm
    {
        [Required(ErrorMessage = "Supplier Invoice ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Supplier Invoice ID must be a positive integer.")]
        public int SupplierInvoiceID { get; set; }

        [Required(ErrorMessage = "Payment status (IsPayment) is required.")]
        public bool IsPayment { get; set; }

        [Required(ErrorMessage = "Product returns are required.")]
        [MinLength(1, ErrorMessage = "At least one product return is required.")]
        public List<ProductReturnDetail> ProductReturns { get; set; }

        public PurchaseReturnConfirm()
        {
            ProductReturns = new List<ProductReturnDetail>();
        }
    }

    public class ProductReturnDetail
    {
        [Required(ErrorMessage = "Product ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Product ID must be a positive integer.")]
        public int ProductID { get; set; }

        [Required(ErrorMessage = "Return quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Return quantity must be a positive integer.")]
        public int ReturnQuantity { get; set; }
    }
}
