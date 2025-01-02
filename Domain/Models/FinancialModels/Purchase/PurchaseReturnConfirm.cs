using System.Collections.Generic;

namespace Domain.Models.FinancialModels
{
    public class PurchaseReturnConfirm
    {
        public int SupplierInvoiceID { get; set; }
        public bool IsPayment { get; set; }
        public List<ProductReturnDetail> ProductReturns { get; set; }

        public PurchaseReturnConfirm()
        {
            ProductReturns = new List<ProductReturnDetail>();
        }
    }

    public class ProductReturnDetail
    {
        public int ProductID { get; set; }
        public int ReturnQuantity { get; set; }
    }
}
