﻿namespace Domain.Models
{
    public class SaleReturnConfirmResult
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public string? InvoiceNo { get; set; }
    }
}
