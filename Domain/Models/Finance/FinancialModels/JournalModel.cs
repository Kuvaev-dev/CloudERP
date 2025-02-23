namespace Domain.Models.FinancialModels
{
    public class JournalModel
    {
        public int SNO { get; set; }    // Selected №
        public DateTime TransectionDate { get; set; }
        public string? AccountSubControl { get; set; }
        public string? TransectionTitle { get; set; }
        public int AccountSubControlID { get; set; }
        public string? InvoiceNo { get; set; }
        public double Debit { get; set; }
        public double Credit { get; set; }
    }
}
