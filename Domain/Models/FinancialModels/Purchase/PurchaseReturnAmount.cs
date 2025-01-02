namespace Domain.Models.FinancialModels
{
    public class PurchaseReturnAmount
    {
        public int InvoiceId { get; set; }
        public float PreviousRemainingAmount { get; set; }
        public float PaymentAmount { get; set; }
    }
}
