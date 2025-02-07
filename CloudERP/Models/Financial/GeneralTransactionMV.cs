namespace CloudERP.Models
{
    public class GeneralTransactionMV
    {
        public int DebitAccountControlID { get; set; }
        public int CreditAccountControlID { get; set; }
        public float TransferAmount { get; set; }
        public string Reason { get; set; }
    }
}