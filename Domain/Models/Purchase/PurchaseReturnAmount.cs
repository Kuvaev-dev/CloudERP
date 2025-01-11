using System.ComponentModel.DataAnnotations;

namespace Domain.Models.FinancialModels
{
    public class PurchaseReturnAmount
    {
        [Required(ErrorMessage = "Invoice ID is required.")]
        public int InvoiceId { get; set; }

        [Required(ErrorMessage = "Previous Remaining Amount is required.")]
        [Range(0, float.MaxValue, ErrorMessage = "Previous Remaining Amount must be a positive value.")]
        public float PreviousRemainingAmount { get; set; }

        [Required(ErrorMessage = "Payment Amount is required.")]
        [Range(0, float.MaxValue, ErrorMessage = "Payment Amount must be a positive value.")]
        public float PaymentAmount { get; set; }
    }
}