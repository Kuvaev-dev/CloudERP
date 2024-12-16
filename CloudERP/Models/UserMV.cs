using System.ComponentModel.DataAnnotations;

namespace CloudERP.Models
{
    public class UserMV
    {
        public int UserID { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(15)]
        [Phone]
        public string ContactNo { get; set; }

        [Required]
        [StringLength(50)]
        public string UserName { get; set; }

        [Required]
        public int UserTypeID { get; set; }

        public string UserTypeName { get; set; }

        public bool IsActive { get; set; }

        public string BranchName { get; set; }

        public string Password { get; set; }
        public string Salt { get; set; }
    }
}