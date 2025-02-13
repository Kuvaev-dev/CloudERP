using Domain.Models;
using System.Collections.Generic;

namespace API.Models
{
    public class ReturnPurchaseInvoiceMV
    {
        public string ReturnInvoiceNo { get; set; }
        public string ReturnInvoiceDate { get; set; }
        public List<SupplierReturnInvoiceDetail> ReturnItems { get; set; }
    }
}