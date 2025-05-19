using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class Customer
    {
        [Key]
        public int CustomerID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [StringLength(150, ErrorMessageResourceName = "StringLengthMaxValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public string? Customername { get; set; }

        [Required(ErrorMessageResourceName = "RequiredValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [StringLength(15, ErrorMessageResourceName = "StringLengthMaxValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [Phone(ErrorMessageResourceName = "PhoneValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public string? CustomerContact { get; set; }

        [StringLength(250, ErrorMessageResourceName = "StringLengthMaxValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public string? CustomerArea { get; set; }

        [StringLength(250, ErrorMessageResourceName = "StringLengthMaxValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public string? CustomerAddress { get; set; }

        [StringLength(500, ErrorMessageResourceName = "StringLengthMaxValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public string? Description { get; set; }
        public int BranchID { get; set; }
        public string? BranchName { get; set; }
        public int CompanyID { get; set; }
        public string? CompanyName { get; set; }
        public int UserID { get; set; }
        public string? UserName { get; set; }
    }
}