using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class PurchaseReturn
    {
        [Required(ErrorMessageResourceName = "RequiredValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [Range(1, int.MaxValue, ErrorMessageResourceName = "RangeValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public int InvoiceId { get; set; }

        [Range(0, float.MaxValue, ErrorMessageResourceName = "RangeNonNegativeValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public float PreviousRemainingAmount { get; set; }

        [Range(0, float.MaxValue, ErrorMessageResourceName = "RangeNonNegativeValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public float PaymentAmount { get; set; }
    }
}