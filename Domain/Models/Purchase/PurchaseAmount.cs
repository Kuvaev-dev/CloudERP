using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class PurchaseAmount
    {
        [Required(ErrorMessageResourceName = "RequiredValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [Range(1, int.MaxValue, ErrorMessageResourceName = "RangeValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public int InvoiceId { get; set; }

        [Range(0, float.MaxValue, ErrorMessageResourceName = "RangeNonNegativeValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public float PreviousRemainingAmount { get; set; }

        [Range(0, float.MaxValue, ErrorMessageResourceName = "RangeNonNegativeValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public float PaidAmount { get; set; }
        public int CompanyID { get; set; }
        public int BranchID { get; set; }
        public int UserID { get; set; }
    }
}