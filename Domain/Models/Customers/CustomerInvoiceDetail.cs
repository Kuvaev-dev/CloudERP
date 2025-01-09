using System.Data;

namespace Domain.Models
{
    public class CustomerInvoiceDetail
    {
        public int CustomerInvoiceDetailID { get; set; }
        public int CustomerInvoiceID { get; set; }
        public int ProductID { get; set; }
        public int SaleQuantity { get; set; }
        public double SaleUnitPrice { get; set; }
        public string CustomerName { get; set; }
        public string CustomerContact { get; set; }
        public string ProductName { get; set; }
        public CustomerInvoice CustomerInvoice { get; set; }
        public CustomerReturnInvoiceDetail CustomerReturnInvoiceDetail { get; set; }
    }
}
