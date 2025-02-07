using Domain.Models;
using System.Collections.Generic;

namespace CloudERP.Models
{
    public class ReturnSaleInvoiceMV
    {
        public string ReturnInvoiceNo { get; set; }
        public string ReturnInvoiceDate { get; set; }
        public List<CustomerReturnInvoiceDetail> ReturnItems { get; set; }
    }
}