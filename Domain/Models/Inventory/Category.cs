using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [StringLength(100, ErrorMessageResourceName = "StringLengthMaxValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public string? CategoryName { get; set; }
        public int BranchID { get; set; }
        public string? BranchName { get; set; }
        public int CompanyID { get; set; }
        public string? CompanyName { get; set; }
        public int UserID { get; set; }
        public string? UserName { get; set; }
    }
}