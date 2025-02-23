using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class AccountActivity
    {
        [Key]
        public int AccountActivityID { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string? Name { get; set; }
    }
}
