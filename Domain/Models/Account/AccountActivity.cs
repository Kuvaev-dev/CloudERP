using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class AccountActivity
    {
        [Key]
        public int AccountActivityID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [StringLength(100, ErrorMessageResourceName = "StringLengthMaxValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public string? Name { get; set; }
    }
}
