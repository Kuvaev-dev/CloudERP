using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class CustomerInvoiceDetail
    {
        [Key]
        public int CustomerInvoiceDetailID { get; set; }

        [Required(ErrorMessage = "Customer Invoice ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Customer Invoice ID must be a positive integer.")]
        public int CustomerInvoiceID { get; set; }

        [Required(ErrorMessage = "Product ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Product ID must be a positive integer.")]
        public int ProductID { get; set; }

        [Required(ErrorMessage = "Sale Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Sale Quantity must be greater than zero.")]
        public int SaleQuantity { get; set; }

        [Required(ErrorMessage = "Sale Unit Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Sale Unit Price must be greater than zero.")]
        public double SaleUnitPrice { get; set; }

        [StringLength(150, ErrorMessage = "Customer Name cannot exceed 150 characters.")]
        public string CustomerName { get; set; }

        [StringLength(150, ErrorMessage = "Customer Area cannot exceed 150 characters.")]
        public string CustomerArea { get; set; }

        [StringLength(15, ErrorMessage = "Customer Contact cannot exceed 15 characters.")]
        public string CustomerContact { get; set; }

        [StringLength(150, ErrorMessage = "Product Name cannot exceed 150 characters.")]
        public string ProductName { get; set; }
        public string CompanyName { get; set; }
        public string BranchName { get; set; }
        public string BranchContact { get; set; }
        public string BranchAddress { get; set; }
        public string CustomerInvoiceNo { get; set; }
        public DateTime CustomerInvoiceDate { get; set; }
        public int ReturnedQuantity { get; set; }
        public int Qty { get; set; }
        public double ItemCost { get; set; }

        public CustomerInvoice CustomerInvoice { get; set; }
        public IEnumerable<CustomerReturnInvoiceDetail> CustomerReturnInvoiceDetail { get; set; }
    }
}