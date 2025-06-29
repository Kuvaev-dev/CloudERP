﻿namespace Domain.Models
{
    public class PurchaseInfo
    {
        public int PaymentID { get; set; }
        public int SupplierID { get; set; }
        public string? SupplierName { get; set; }
        public string? SupplierContactNo { get; set; }
        public string? SupplierAddress { get; set; }
        public int SupplierInvoiceID { get; set; }
        public int CompanyID { get; set; }
        public int BranchID { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string? InvoiceNo { get; set; }
        public double TotalAmount { get; set; }
        public double ReturnProductAmount { get; set; }
        public double AfterReturnTotalAmount { get; set; }
        public double PaymentAmount { get; set; }
        public double ReturnPaymentAmount { get; set; }
        public double RemainingBalance { get; set; }
        public int UserID { get; set; }
        public string? UserName { get; set; }
    }
}
