namespace Domain.Models.FinancialModels
{
    public class SalePayment
    {
        public int InvoiceId { get; set; }
        public float PreviousRemainingAmount { get; set; }
        public float PaidAmount { get; set; }
    }
}
