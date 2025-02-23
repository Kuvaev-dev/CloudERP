using System;

namespace Domain.Models
{
    public class ProductQuality
    {
        public int ProductID { get; set; }
        public string? ProductName { get; set; }
        public DateTime ExpiryDate { get; set; }
        public DateTime Manufacture { get; set; }
        public int StockTreshHoldQuantity { get; set; }

        public bool IsExpired => DateTime.Now > ExpiryDate;

        public string EvaluateQuality()
        {
            if (IsExpired)
                return "Expired";
            if (StockTreshHoldQuantity > 50)
                return "Low Stock";
            return "Good";
        }
    }
}
