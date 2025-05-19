using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class SupplierReturnInvoiceDetail
    {
        [Key]
        public int SupplierReturnInvoiceDetailID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [Range(1, int.MaxValue, ErrorMessageResourceName = "RangeValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public int SupplierInvoiceID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [Range(1, int.MaxValue, ErrorMessageResourceName = "RangeValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public int SupplierInvoiceDetailID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [Range(1, int.MaxValue, ErrorMessageResourceName = "RangeValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public int SupplierReturnInvoiceID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [Range(1, int.MaxValue, ErrorMessageResourceName = "RangeValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public int ProductID { get; set; }

        [Range(1, int.MaxValue, ErrorMessageResourceName = "RangeMinValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public int PurchaseReturnQuantity { get; set; }

        [Range(0, double.MaxValue, ErrorMessageResourceName = "RangeNonNegativeValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public double PurchaseReturnUnitPrice { get; set; }
        public string? InvoiceNo { get; set; }
        public string? ProductName { get; set; }
        public DateTime InvoiceDate { get; set; }
    }
}