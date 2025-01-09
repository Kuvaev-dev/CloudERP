using System;

namespace Domain.Models
{
    public class Stock
    {
        public int ProductID { get; set; }
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public double SaleUnitPrice { get; set; }
        public double CurrentPurchaseUnitPrice { get; set; }
        public DateTime ExpiryDate { get; set; }
        public DateTime Manufacture { get; set; }
        public int StockTreshHoldQuantity { get; set; }
        public string Description { get; set; }
        public int UserID { get; set; }
        public int BranchID { get; set; }
        public int CompanyID { get; set; }
        public bool? IsActive { get; set; }
    }
}
