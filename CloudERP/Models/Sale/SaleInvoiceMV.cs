using Domain.Models;

namespace CloudERP.Models
{
    public class SaleInvoiceMV
    {
        public string CustomerName { get; set; }
        public string CustomerContact { get; set; }
        public string CustomerArea { get; set; }
        public string CustomerLogo { get; set; }
        public string CompanyName { get; set; }
        public string CompanyLogo { get; set; }
        public string BranchName { get; set; }
        public string BranchContact { get; set; }
        public string BranchAddress { get; set; }
        public string InvoiceNo { get; set; }
        public string InvoiceDate { get; set; }
        public double TotalCost { get; set; }
        public List<CustomerInvoiceDetail> InvoiceItems { get; set; }
        public List<ReturnSaleInvoiceMV> ReturnInvoices { get; set; }
    }
}