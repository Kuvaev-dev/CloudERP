using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class SupplierInvoiceDetail
    {
        [Key]
        public int SupplierInvoiceDetailID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [Range(1, int.MaxValue, ErrorMessageResourceName = "RangeValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public int SupplierInvoiceID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [Range(1, int.MaxValue, ErrorMessageResourceName = "RangeValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public int ProductID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [StringLength(150, ErrorMessageResourceName = "StringLengthMaxValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public string? ProductName { get; set; }

        [Range(0, int.MaxValue, ErrorMessageResourceName = "RangeNonNegativeValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public int PurchaseQuantity { get; set; }

        [Range(0, int.MaxValue, ErrorMessageResourceName = "RangeNonNegativeValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public int SaleQuantity { get; set; }

        [Range(0, double.MaxValue, ErrorMessageResourceName = "RangeNonNegativeValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public double SaleUnitPrice { get; set; }

        [Range(0, double.MaxValue, ErrorMessageResourceName = "RangeNonNegativeValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public double PurchaseUnitPrice { get; set; }

        [Required(ErrorMessageResourceName = "RequiredValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public string? UserName { get; set; }
        public string? CompanyName { get; set; }
        public string? CompanyLogo { get; set; }
        public string? SupplierInvoiceNo { get; set; }
        public DateTime SupplierInvoiceDate { get; set; }

        [Required(ErrorMessageResourceName = "RequiredValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [Range(1, int.MaxValue, ErrorMessageResourceName = "RangeValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public int SaleCartDetailID { get; set; }
        public int ReturnedQuantity { get; set; }
        public int Qty { get; set; }
        public double ItemCost { get; set; }

        public Supplier? Supplier { get; set; }
        public SupplierInvoice? SupplierInvoice { get; set; }
        public Branch? Branch { get; set; }
        public List<SupplierReturnInvoiceDetail>? SupplierReturnInvoiceDetail { get; set; }
        public Stock? Stock { get; set; }
    }
}