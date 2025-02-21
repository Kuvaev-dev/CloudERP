using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class SupplierReturnInvoice
    {
        [Key]
        public int SupplierReturnInvoiceID { get; set; }

        [Required(ErrorMessage = "Supplier Invoice ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Supplier Invoice ID must be a positive integer.")]
        public int SupplierInvoiceID { get; set; }

        [Required(ErrorMessage = "Supplier ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Supplier ID must be a positive integer.")]
        public int SupplierID { get; set; }

        [Required(ErrorMessage = "Company ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Company ID must be a positive integer.")]
        public int CompanyID { get; set; }

        [Required(ErrorMessage = "Branch ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Branch ID must be a positive integer.")]
        public int BranchID { get; set; }

        [Required(ErrorMessage = "Invoice Number is required.")]
        [StringLength(50, ErrorMessage = "Invoice Number cannot be longer than 50 characters.")]
        public string InvoiceNo { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Total Amount must be a positive value.")]
        public double TotalAmount { get; set; }

        [Required(ErrorMessage = "Invoice Date is required.")]
        public DateTime InvoiceDate { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "User ID must be a positive integer.")]
        public int UserID { get; set; }
        public string SupplierName { get; set; }
        public List<SupplierReturnPayment> SupplierReturnPayment { get; set; }
    }
}
