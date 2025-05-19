using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class Stock
    {
        [Key]
        public int ProductID { get; set; }
        public int CategoryID { get; set; }
        public string? CategoryName { get; set; }

        [Required(ErrorMessageResourceName = "RequiredValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [StringLength(200, ErrorMessageResourceName = "StringLengthMaxValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public string? ProductName { get; set; }

        [Required(ErrorMessageResourceName = "RequiredValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [Range(0, int.MaxValue, ErrorMessageResourceName = "RangeNonNegativeValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public int Quantity { get; set; }

        [Required(ErrorMessageResourceName = "RequiredValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [Range(0.01, double.MaxValue, ErrorMessageResourceName = "RangeMinValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public double SaleUnitPrice { get; set; }

        [Required(ErrorMessageResourceName = "RequiredValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [Range(0.01, double.MaxValue, ErrorMessageResourceName = "RangeMinValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public double CurrentPurchaseUnitPrice { get; set; }

        [Required(ErrorMessageResourceName = "RequiredValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [DataType(DataType.Date, ErrorMessageResourceName = "DateValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public DateTime ExpiryDate { get; set; }

        [Required(ErrorMessageResourceName = "RequiredValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [DataType(DataType.Date, ErrorMessageResourceName = "DateValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public DateTime Manufacture { get; set; }

        [Required(ErrorMessageResourceName = "RequiredValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [Range(0, int.MaxValue, ErrorMessageResourceName = "RangeNonNegativeValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public int StockTreshHoldQuantity { get; set; }

        [StringLength(500, ErrorMessageResourceName = "StringLengthMaxValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public string? Description { get; set; }
        public int UserID { get; set; }
        public string? UserName { get; set; }
        public int BranchID { get; set; }
        public string? BranchName { get; set; }
        public int CompanyID { get; set; }
        public string? CompanyName { get; set; }
        public bool? IsActive { get; set; }
    }
}