using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class User
    {
        public int UserID { get; set; }

        [Required(ErrorMessage = "Full Name is required.")]
        [StringLength(100, ErrorMessage = "Full Name cannot exceed 100 characters.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Invalid contact number format.")]
        [StringLength(15, ErrorMessage = "Contact number cannot exceed 15 characters.")]
        public string ContactNo { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 255 characters.")]
        public string Password { get; set; }

        public string Salt { get; set; }

        [Required(ErrorMessage = "User Type ID is required.")]
        public int UserTypeID { get; set; }

        public bool IsActive { get; set; }

        [StringLength(50, ErrorMessage = "User Type Name cannot exceed 50 characters.")]
        public string UserTypeName { get; set; }

        [StringLength(100, ErrorMessage = "Branch Name cannot exceed 100 characters.")]
        public string BranchName { get; set; }

        public string ResetPasswordCode { get; set; }

        public DateTime? LastPasswordResetRequest { get; set; }

        public DateTime? ResetPasswordExpiration { get; set; }
    }
}
