using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class SupportTicket
    {
        public int TicketID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [StringLength(100, ErrorMessageResourceName = "StringLengthMaxValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public string? Subject { get; set; }

        [Required(ErrorMessageResourceName = "RequiredValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [StringLength(50, MinimumLength = 2, ErrorMessageResourceName = "StringLengthMinMaxValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public string? Name { get; set; }

        [Required(ErrorMessageResourceName = "RequiredValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [EmailAddress(ErrorMessageResourceName = "EmailValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [StringLength(100, ErrorMessageResourceName = "StringLengthMaxValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public string? Email { get; set; }

        [Required(ErrorMessageResourceName = "RequiredValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [StringLength(1000, MinimumLength = 10, ErrorMessageResourceName = "StringLengthMinMaxValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public string? Message { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateCreated { get; set; }

        [StringLength(1000, MinimumLength = 10, ErrorMessageResourceName = "StringLengthMinMaxValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public string? AdminResponse { get; set; }

        [StringLength(50, MinimumLength = 2, ErrorMessageResourceName = "StringLengthMinMaxValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public string? RespondedBy { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? ResponseDate { get; set; }

        public bool IsResolved { get; set; }

        [Range(1, int.MaxValue, ErrorMessageResourceName = "RangeValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public int CompanyID { get; set; }

        [Range(1, int.MaxValue, ErrorMessageResourceName = "RangeValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public int BranchID { get; set; }

        [Range(1, int.MaxValue, ErrorMessageResourceName = "RangeValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public int UserID { get; set; }
    }
}