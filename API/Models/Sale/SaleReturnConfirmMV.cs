﻿namespace API.Models
{
    public class SaleReturnConfirmMV
    {
        public int CustomerInvoiceID { get; set; }
        public bool IsPayment { get; set; }
        public List<ProductReturnItem> ProductReturns { get; set; }
    }
}