namespace Domain.Models
{
    public class SaleReturnConfirm
    {
        public List<int>? ProductIDs { get; set; }
        public List<int>? ReturnQty { get; set; }
        public int CustomerInvoiceID { get; set; }
        public bool IsPayment { get; set; }
        public int CompanyID { get; set; }
        public int BranchID { get; set; }
        public int UserID { get; set; }
    }
}
