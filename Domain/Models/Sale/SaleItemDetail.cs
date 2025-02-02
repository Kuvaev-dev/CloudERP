using System.Collections.Generic;

namespace Domain.Models.Sale
{
    public class SaleItemDetailDto
    {
        public string InvoiceNo { get; set; }
        public List<SaleProductDetail> Products { get; set; }
        public double Total { get; set; }
        public List<SaleReturnDetail> Returns { get; set; }
    }

    public class SaleProductDetail
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double ItemCost { get; set; }
    }

    public class SaleReturnDetail
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double ItemCost { get; set; }
    }
}
