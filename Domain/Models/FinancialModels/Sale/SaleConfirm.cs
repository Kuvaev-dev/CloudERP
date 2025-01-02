namespace Domain.Models.FinancialModels
{
    public class SaleConfirm
    {
        public int CustomerID { get; set; }
        public string Description { get; set; }
        public bool IsPayment { get; set; }
    }
}
