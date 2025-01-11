using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class SupplierInvoice
    {
        public int SupplierInvoiceID { get; set; }

        [Required(ErrorMessage = "Supplier ID is required.")]
        public int SupplierID { get; set; }

        [Required(ErrorMessage = "Supplier Name is required.")]
        [StringLength(100, ErrorMessage = "Supplier Name cannot be longer than 100 characters.")]
        public string SupplierName { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format.")]
        [StringLength(15, ErrorMessage = "Supplier Contact No cannot be longer than 15 characters.")]
        public string SupplierConatctNo { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        [StringLength(100, ErrorMessage = "Supplier Email cannot be longer than 100 characters.")]
        public string SupplierEmail { get; set; }

        [Required(ErrorMessage = "Company ID is required.")]
        public int CompanyID { get; set; }

        [StringLength(250, ErrorMessage = "Company Logo cannot be longer than 250 characters.")]
        public string CompanyLogo { get; set; }

        [Required(ErrorMessage = "Company Name is required.")]
        [StringLength(150, ErrorMessage = "Company Name cannot be longer than 150 characters.")]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "Branch ID is required.")]
        public int BranchID { get; set; }

        [StringLength(150, ErrorMessage = "Branch Name cannot be longer than 150 characters.")]
        public string BranchName { get; set; }

        [StringLength(250, ErrorMessage = "Branch Address cannot be longer than 250 characters.")]
        public string BranchAddress { get; set; }

        [Phone(ErrorMessage = "Invalid branch contact number format.")]
        [StringLength(15, ErrorMessage = "Branch Contact cannot be longer than 15 characters.")]
        public string BranchContact { get; set; }

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
        public int UserID { get; set; }

        [Required(ErrorMessage = "User Name is required.")]
        [StringLength(100, ErrorMessage = "User Name cannot be longer than 100 characters.")]
        public string UserName { get; set; }

        public IEnumerable<SupplierInvoiceDetail> SupplierInvoices { get; set; }
    }
}
