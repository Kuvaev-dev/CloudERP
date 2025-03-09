using Domain.Models;

namespace CloudERP.Models
{
    public class PurchaseInvoiceMV
    {
        public string SupplierName { get; set; }
        public string SupplierConatctNo { get; set; }
        public string SupplierAddress { get; set; }
        public string SupplierLogo { get; set; }
        public string CompanyName { get; set; }
        public string CompanyLogo { get; set; }
        public string BranchName { get; set; }
        public string BranchContact { get; set; }
        public string BranchAddress { get; set; }
        public string InvoiceNo { get; set; }
        public string InvoiceDate { get; set; }
        public double TotalCost { get; set; }
        public List<SupplierInvoiceDetail> InvoiceItems { get; set; }
        public List<ReturnPurchaseInvoiceMV> ReturnInvoices { get; set; }
    }
}