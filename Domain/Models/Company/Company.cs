using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class Company
    {
        [Key]
        public int CompanyID { get; set; }
        public string? Name { get; set; }
        public string? Logo { get; set; }
        public string? Description { get; set; }
    }
}
