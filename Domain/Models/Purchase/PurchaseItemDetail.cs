using System.Collections.Generic;

namespace Domain.Models.Purchase
{
    public class PurchaseItemDetailDto
    {
        public string InvoiceNo { get; set; }
        public List<PurchaseProductDetail> Products { get; set; }
        public double Total { get; set; }
        public List<PurchaseReturnDetail> Returns { get; set; }
    }

    public class PurchaseProductDetail
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double ItemCost { get; set; }
    }

    public class PurchaseReturnDetail
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double ItemCost { get; set; }
    }
}
