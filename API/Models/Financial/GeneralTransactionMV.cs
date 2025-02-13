namespace API.Models
{
    public class GeneralTransactionMV
    {
        public int DebitAccountControlID { get; set; }
        public int CreditAccountControlID { get; set; }
        public int BranchID { get; set; }
        public int CompanyID { get; set; }
        public int UserID { get; set; }
        public float TransferAmount { get; set; }
        public string Reason { get; set; }
    }
}