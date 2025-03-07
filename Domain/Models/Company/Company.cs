using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class Company
    {
        [Key]
        public int CompanyID { get; set; }
        [Required(ErrorMessage = "Company Name is required.")]
        [StringLength(100, ErrorMessage = "Company Name cannot exceed 100 characters.")]
        public string? Name { get; set; }
        public string? Logo { get; set; }
        public string? Description { get; set; }
    }
}
