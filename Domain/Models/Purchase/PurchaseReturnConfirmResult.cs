﻿namespace Domain.Models
{
    public class PurchaseReturnConfirmResult
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public string? InvoiceNo { get; set; }
    }
}
