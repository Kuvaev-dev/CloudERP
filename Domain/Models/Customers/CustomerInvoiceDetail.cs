using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class CustomerInvoiceDetail
    {
        [Key]
        public int CustomerInvoiceDetailID { get; set; }

        [Required(ErrorMessage = "Customer Invoice ID is required.")]
        public int CustomerInvoiceID { get; set; }

        [Required(ErrorMessage = "Product ID is required.")]
        public int ProductID { get; set; }

        [Required(ErrorMessage = "Sale Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Sale Quantity must be greater than zero.")]
        public int SaleQuantity { get; set; }

        [Required(ErrorMessage = "Sale Unit Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Sale Unit Price must be greater than zero.")]
        public double SaleUnitPrice { get; set; }

        [StringLength(150, ErrorMessage = "Customer Name cannot exceed 150 characters.")]
        public string CustomerName { get; set; }

        [StringLength(15, ErrorMessage = "Customer Contact cannot exceed 15 characters.")]
        public string CustomerContact { get; set; }

        [StringLength(150, ErrorMessage = "Product Name cannot exceed 150 characters.")]
        public string ProductName { get; set; }

        public CustomerInvoice CustomerInvoice { get; set; }

        public CustomerReturnInvoiceDetail CustomerReturnInvoiceDetail { get; set; }
    }
}