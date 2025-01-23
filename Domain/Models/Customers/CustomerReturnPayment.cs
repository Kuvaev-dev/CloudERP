using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class CustomerReturnPayment
    {
        [Key]
        public int CustomerReturnPaymentID { get; set; }

        [Required(ErrorMessage = "Customer Return Invoice ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Customer Return Invoice ID must be a positive integer.")]
        public int CustomerReturnInvoiceID { get; set; }

        [Required(ErrorMessage = "Customer ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Customer ID must be a positive integer.")]
        public int CustomerID { get; set; }

        [Required(ErrorMessage = "Customer Name is required.")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Customer Invoice ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Customer Invoice ID must be a positive integer.")]
        public int CustomerInvoiceID { get; set; }

        [Required(ErrorMessage = "Company ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Company ID must be a positive integer.")]
        public int CompanyID { get; set; }

        [Required(ErrorMessage = "Branch ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Branch ID must be a positive integer.")]
        public int BranchID { get; set; }

        [Required(ErrorMessage = "Invoice Number is required.")]
        [StringLength(50, ErrorMessage = "Invoice Number cannot exceed 50 characters.")]
        public string InvoiceNo { get; set; }

        [Required(ErrorMessage = "Total Amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Total Amount must be greater than zero.")]
        public double TotalAmount { get; set; }

        [Required(ErrorMessage = "Paid Amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Paid Amount must be greater than zero.")]
        public double PaidAmount { get; set; }

        [Required(ErrorMessage = "Remaining Balance is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Remaining Balance must be greater than zero.")]
        public double RemainingBalance { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "User ID must be a positive integer.")]
        public int UserID { get; set; }

        [Required(ErrorMessage = "User Name is required.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Invoice Date is required.")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
        public DateTime InvoiceDate { get; set; }
    }
}