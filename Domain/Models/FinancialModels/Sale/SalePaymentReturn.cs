namespace Domain.Models.FinancialModels
{
    public class SalePaymentReturn
    {
        public int InvoiceId { get; set; }
        public float PreviousRemainingAmount { get; set; }
        public float PaymentAmount { get; set; }
    }
}
