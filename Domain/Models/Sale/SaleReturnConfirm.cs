using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models.FinancialModels
{
    public class SaleReturnConfirm
    {
        [Required(ErrorMessage = "Product IDs are required.")]
        public List<int> ProductIDs { get; set; }

        [Required(ErrorMessage = "Return quantities are required.")]
        public List<int> ReturnQty { get; set; }

        [Required(ErrorMessage = "Customer Invoice ID is required.")]
        public int CustomerInvoiceID { get; set; }

        [Required(ErrorMessage = "Payment status is required.")]
        public bool IsPayment { get; set; }
    }
}
