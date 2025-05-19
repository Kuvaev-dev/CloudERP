using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class CustomerInvoiceDetail
    {
        [Key]
        public int CustomerInvoiceDetailID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [Range(1, int.MaxValue, ErrorMessageResourceName = "RangeValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public int CustomerInvoiceID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [Range(1, int.MaxValue, ErrorMessageResourceName = "RangeValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public int ProductID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [Range(1, int.MaxValue, ErrorMessageResourceName = "RangeMinValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public int SaleQuantity { get; set; }

        [Required(ErrorMessageResourceName = "RequiredValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [Range(0.01, double.MaxValue, ErrorMessageResourceName = "RangeMinValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public double SaleUnitPrice { get; set; }

        [StringLength(150, ErrorMessageResourceName = "StringLengthMaxValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public string? ProductName { get; set; }
        public string? CompanyName { get; set; }
        public string? CompanyLogo { get; set; }
        public string? CustomerInvoiceNo { get; set; }
        public DateTime CustomerInvoiceDate { get; set; }
        public int ReturnedQuantity { get; set; }
        public int Qty { get; set; }
        public double ItemCost { get; set; }
        public double TotalCost { get; set; }

        public CustomerInvoice? CustomerInvoice { get; set; }
        public Customer? Customer { get; set; }
        public Branch? Branch { get; set; }
        public List<CustomerInvoiceDetail>? CustomerInvoiceDetails { get; set; }
        public List<CustomerReturnInvoice>? ReturnInvoices { get; set; }
        public List<CustomerReturnInvoiceDetail>? CustomerReturnInvoiceDetail { get; set; }
    }
}