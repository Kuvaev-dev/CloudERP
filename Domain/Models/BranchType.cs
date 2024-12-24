using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class BranchType
    {
        [Key]
        public int BranchTypeID { get; set; }

        [Required(ErrorMessage = "Branch Type Name is required.")]
        [StringLength(50, ErrorMessage = "Branch Type Name cannot exceed 50 characters.")]
        public string BranchTypeName { get; set; }
    }
}
