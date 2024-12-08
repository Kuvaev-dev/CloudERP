using System.ComponentModel.DataAnnotations;

namespace CloudERP.Models
{
    public class UserTypeMV
    {
        public int UserTypeID { get; set; }

        [Required(ErrorMessage = "User Type is required.")]
        [StringLength(50, ErrorMessage = "User Type must not exceed 50 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Only letters and spaces are allowed.")]
        public string UserTypeName { get; set; }
    }
}