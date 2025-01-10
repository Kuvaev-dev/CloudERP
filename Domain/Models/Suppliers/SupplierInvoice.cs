using System.Collections.Generic;

namespace Domain.Models
{
    public class SupplierInvoice
    {
        public int SupplierInvoiceID { get; set; }
        public int SupplierID { get; set; }
        public string SupplierName { get; set; }
        public string SupplierConatctNo { get; set; }
        public string SupplierEmail { get; set; }
        public int CompanyID { get; set; }
        public string CompanyLogo { get; set; }
        public string CompanyName { get; set; }
        public int BranchID { get; set; }
        public string BranchName { get; set; }
        public string BranchAddress { get; set; }
        public string BranchContact { get; set; }
        public string InvoiceNo { get; set; }
        public double TotalAmount { get; set; }
        public System.DateTime InvoiceDate { get; set; }
        public string Description { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }
        public IEnumerable<SupplierInvoiceDetail> SupplierInvoices { get; set; }
    }
}
