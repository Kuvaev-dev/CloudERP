using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models.FinancialModels
{
    public class SalePaymentModel
    {
        [Required(ErrorMessage = "Customer Payment ID is required.")]
        public int CustomerPaymentID { get; set; }

        [Required(ErrorMessage = "Customer ID is required.")]
        public int CustomerID { get; set; }

        [Required(ErrorMessage = "Customer Name is required.")]
        [StringLength(100, ErrorMessage = "Customer Name cannot exceed 100 characters.")]
        public string CustomerName { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format.")]
        [StringLength(15, ErrorMessage = "Customer Contact Number cannot exceed 15 characters.")]
        public string CustomerContactNo { get; set; }

        [StringLength(200, ErrorMessage = "Customer Address cannot exceed 200 characters.")]
        public string CustomerAddress { get; set; }

        [Required(ErrorMessage = "Customer Invoice ID is required.")]
        public int CustomerInvoiceID { get; set; }

        [Required(ErrorMessage = "Company ID is required.")]
        public int CompanyID { get; set; }

        [Required(ErrorMessage = "Branch ID is required.")]
        public int BranchID { get; set; }

        [Required(ErrorMessage = "Invoice Date is required.")]
        public DateTime InvoiceDate { get; set; }

        [Required(ErrorMessage = "Invoice Number is required.")]
        [StringLength(50, ErrorMessage = "Invoice Number cannot exceed 50 characters.")]
        public string InvoiceNo { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Total Amount must be a positive value.")]
        public double TotalAmount { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Return Product Amount must be a positive value.")]
        public double ReturnProductAmount { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "After Return Total Amount must be a positive value.")]
        public double AfterReturnTotalAmount { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Payment Amount must be a positive value.")]
        public double PaymentAmount { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Return Payment Amount must be a positive value.")]
        public double ReturnPaymentAmount { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Remaining Balance must be a positive value.")]
        public double RemainingBalance { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        public int UserID { get; set; }

        [Required(ErrorMessage = "User Name is required.")]
        [StringLength(50, ErrorMessage = "User Name cannot exceed 50 characters.")]
        public string UserName { get; set; }
    }
}
