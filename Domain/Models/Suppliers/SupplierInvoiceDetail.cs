namespace Domain.Models
{
    public class SupplierInvoiceDetail
    {
        public int SupplierInvoiceDetailID { get; set; }
        public int SupplierInvoiceID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int PurchaseQuantity { get; set; }
        public int SaleQuantity { get; set; }
        public double SaleUnitPrice { get; set; }
        public double PurchaseUnitPrice { get; set; }
        public double UserName { get; set; }
    }
}
