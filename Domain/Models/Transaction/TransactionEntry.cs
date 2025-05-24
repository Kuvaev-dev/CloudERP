namespace Domain.Models
{
    public class TransactionEntry
    {
        public string FinancialYearID { get; set; }
        public string AccountHeadID { get; set; }
        public string AccountControlID { get; set; }
        public string AccountSubControlID { get; set; }
        public string InvoiceNo { get; set; }
        public string UserID { get; set; }
        public float Credit { get; set; }
        public float Debit { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TransactionTitle { get; set; }
    }
}