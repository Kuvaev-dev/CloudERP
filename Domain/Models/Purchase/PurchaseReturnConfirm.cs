using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models.FinancialModels
{
    public class PurchaseReturnConfirm
    {
        [Required(ErrorMessage = "Product IDs are required.")]
        public List<int>? ProductIDs { get; set; }

        [Required(ErrorMessage = "Return quantities are required.")]
        public List<int>? ReturnQty { get; set; }

        [Required(ErrorMessage = "Supplier Invoice ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Supplier Invoice ID must be a positive integer.")]
        public int SupplierInvoiceID { get; set; }

        [Required(ErrorMessage = "Payment status is required.")]
        public bool IsPayment { get; set; }
        public int CompanyID { get; set; }
        public int BranchID { get; set; }
        public int UserID { get; set; }
    }
}
