using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class SupportTicket
    {
        public int TicketID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [StringLength(100, ErrorMessageResourceName = "StringLengthMaxValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public string? Subject { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }

        [Required(ErrorMessageResourceName = "RequiredValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [StringLength(1000, MinimumLength = 10, ErrorMessageResourceName = "StringLengthMinMaxValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public string? Message { get; set; }
        public DateTime DateCreated { get; set; }
        public string? AdminResponse { get; set; }
        public string? RespondedBy { get; set; }
        public DateTime? ResponseDate { get; set; }
        public bool IsResolved { get; set; }
        public int CompanyID { get; set; }
        public int BranchID { get; set; }
        public int UserID { get; set; }
    }
}