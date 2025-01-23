using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models.FinancialModels
{
    public class PurchasePaymentModel
    {
        [Key]
        public int SupplierPaymentID { get; set; }

        [Required(ErrorMessage = "Supplier ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Supplier ID must be a positive integer.")]
        public int SupplierID { get; set; }

        [Required(ErrorMessage = "Supplier Name is required.")]
        [StringLength(100, ErrorMessage = "Supplier Name cannot exceed 100 characters.")]
        public string SupplierName { get; set; }

        [StringLength(15, ErrorMessage = "Supplier Contact Number cannot exceed 15 characters.")]
        public string SupplierContactNo { get; set; }

        [StringLength(250, ErrorMessage = "Supplier Address cannot exceed 250 characters.")]
        public string SupplierAddress { get; set; }

        [Required(ErrorMessage = "Supplier Invoice ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Supplier Invoice ID must be a positive integer.")]
        public int SupplierInvoiceID { get; set; }

        [Required(ErrorMessage = "Company ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Company ID must be a positive integer.")]
        public int CompanyID { get; set; }

        [Required(ErrorMessage = "Branch ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Branch ID must be a positive integer.")]
        public int BranchID { get; set; }

        [Required(ErrorMessage = "Invoice Date is required.")]
        public DateTime InvoiceDate { get; set; }

        [Required(ErrorMessage = "Invoice Number is required.")]
        [StringLength(50, ErrorMessage = "Invoice Number cannot exceed 50 characters.")]
        public string InvoiceNo { get; set; }

        [Required(ErrorMessage = "Total Amount is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Total Amount must be a positive value.")]
        public double TotalAmount { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Return Product Amount must be a positive value.")]
        public double ReturnProductAmount { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "After Return Total Amount must be a positive value.")]
        public double AfterReturnTotalAmount { get; set; }

        [Required(ErrorMessage = "Payment Amount is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Payment Amount must be a positive value.")]
        public double PaymentAmount { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Return Payment Amount must be a positive value.")]
        public double ReturnPaymentAmount { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Remaining Balance must be a positive value.")]
        public double RemainingBalance { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "User ID must be a positive integer.")]
        public int UserID { get; set; }

        [Required(ErrorMessage = "User Name is required.")]
        [StringLength(100, ErrorMessage = "User Name cannot exceed 100 characters.")]
        public string UserName { get; set; }
    }
}
