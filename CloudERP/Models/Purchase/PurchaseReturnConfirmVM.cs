namespace CloudERP.Models
{
    public class PurchaseReturnConfirmVM
    {
        public int SupplierInvoiceID { get; set; }
        public bool IsPayment { get; set; }
        public List<ProductReturnItem> ProductReturns { get; set; }
    }
}