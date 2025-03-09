using Domain.Models;

namespace CloudERP.Models
{
    public class ReturnPurchaseInvoiceMV
    {
        public string ReturnInvoiceNo { get; set; }
        public string ReturnInvoiceDate { get; set; }
        public List<SupplierReturnInvoiceDetail> ReturnItems { get; set; }
    }
}