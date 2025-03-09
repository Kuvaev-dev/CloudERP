using Domain.Models;

namespace CloudERP.Models
{
    public class ReturnSaleInvoiceMV
    {
        public string ReturnInvoiceNo { get; set; }
        public string ReturnInvoiceDate { get; set; }
        public List<CustomerReturnInvoiceDetail> ReturnItems { get; set; }
    }
}