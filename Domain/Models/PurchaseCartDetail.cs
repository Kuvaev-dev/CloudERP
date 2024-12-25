namespace Domain.Models
{
    public class PurchaseCartDetail
    {
        public int PurchaseCartDetailID { get; set; }
        public int ProductID { get; set; }
        public int PurchaseQuantity { get; set; }
        public double PurchaseUnitPrice { get; set; }
        public int CompanyID { get; set; }
        public int BranchID { get; set; }
        public int UserID { get; set; }
    }
}
