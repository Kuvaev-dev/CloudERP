using System.Collections.Generic;

namespace CloudERP.Models
{
    public class ReturnConfirmVM
    {
        public int CustomerInvoiceID { get; set; }
        public bool IsPayment { get; set; }
        public List<ProductReturnItem> ProductReturns { get; set; }
    }

    public class ProductReturnItem
    {
        public int ProductID { get; set; }
        public int ReturnQty { get; set; }
    }
}