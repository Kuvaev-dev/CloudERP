using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class CustomerReturnInvoiceDetail
    {
        [Key]
        public int CustomerReturnInvoiceDetailID { get; set; }

        [Required(ErrorMessage = "Customer Invoice Detail ID is required.")]
        public int CustomerInvoiceDetailID { get; set; }

        [Required(ErrorMessage = "Customer Invoice ID is required.")]
        public int CustomerInvoiceID { get; set; }

        [Required(ErrorMessage = "Customer Return Invoice ID is required.")]
        public int CustomerReturnInvoiceID { get; set; }

        [Required(ErrorMessage = "Product ID is required.")]
        public int ProductID { get; set; }

        [Required(ErrorMessage = "Sale Return Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Sale Return Quantity must be greater than zero.")]
        public int SaleReturnQuantity { get; set; }

        [Required(ErrorMessage = "Sale Return Unit Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Sale Return Unit Price must be greater than zero.")]
        public double SaleReturnUnitPrice { get; set; }
    }
}