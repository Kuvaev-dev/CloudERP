using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class UserType
    {
        public int UserTypeID { get; set; }

        [Required(ErrorMessage = "User Type Name is required.")]
        [StringLength(50, ErrorMessage = "User Type Name cannot exceed 50 characters.")]
        public string UserTypeName { get; set; }
    }
}
