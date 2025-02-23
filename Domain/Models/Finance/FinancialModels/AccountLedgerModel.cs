namespace Domain.Models.FinancialModels
{
    public class AccountLedgerModel
    {
        public int SNo { get; set; }
        public string? Account { get; set; }
        public string? Date { get; set; }
        public string? Description { get; set; }
        public string? Debit { get; set; }
        public string? Credit { get; set; }
    }
}
