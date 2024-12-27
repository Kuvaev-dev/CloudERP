namespace Domain.Models
{
    public class CustomerReturnInvoiceDetail
    {
        public int CustomerReturnInvoiceDetailID { get; set; }
        public int CustomerInvoiceDetailID { get; set; }
        public int CustomerInvoiceID { get; set; }
        public int CustomerReturnInvoiceID { get; set; }
        public int ProductID { get; set; }
        public int SaleReturnQuantity { get; set; }
        public double SaleReturnUnitPrice { get; set; }
    }
}
