namespace Domain.Models
{
    public class SupplierReturnInvoiceDetail
    {
        public int SupplierReturnInvoiceDetailID { get; set; }
        public int SupplierInvoiceID { get; set; }
        public int SupplierInvoiceDetailID { get; set; }
        public int SupplierReturnInvoiceID { get; set; }
        public int ProductID { get; set; }
        public int PurchaseReturnQuantity { get; set; }
        public double PurchaseReturnUnitPrice { get; set; }
    }
}
