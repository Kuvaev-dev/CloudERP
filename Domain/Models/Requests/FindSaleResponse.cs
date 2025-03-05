namespace Domain.Models
{
    public class FindSaleResponse
    {
        public CustomerInvoice Invoice { get; set; }
        public List<CustomerInvoiceDetail> InvoiceDetails { get; set; }
    }
}
