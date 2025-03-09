using Domain.Models;

namespace API.Models
{
    public class ReturnSaleInvoiceMV
    {
        public string ReturnInvoiceNo { get; set; }
        public string ReturnInvoiceDate { get; set; }
        public List<CustomerReturnInvoiceDetail> ReturnItems { get; set; }
    }
}