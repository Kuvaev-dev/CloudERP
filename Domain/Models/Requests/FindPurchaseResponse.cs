namespace Domain.Models
{
    public class FindPuchaseResponse
    {
        public SupplierInvoice Invoice { get; set; }
        public List<SupplierInvoiceDetail> InvoiceDetails { get; set; }
    }
}
