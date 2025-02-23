using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class Branch
    {
        [Key]
        public int BranchID { get; set; }
        public int BrchID { get; set; }

        [Required(ErrorMessage = "Branch Name is required.")]
        [StringLength(100, ErrorMessage = "Branch Name cannot exceed 100 characters.")]
        public string? BranchName { get; set; }

        [StringLength(50, ErrorMessage = "Branch Contact cannot exceed 50 characters.")]
        public string? BranchContact { get; set; }

        [StringLength(500, ErrorMessage = "Branch Address cannot exceed 200 characters.")]
        public string? BranchAddress { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        [Required(ErrorMessage = "Company ID is required.")]
        public int CompanyID { get; set; }
        public int? ParentBranchID { get; set; }
        public int BranchTypeID { get; set; }
        public string? BranchTypeName { get; set; }
    }
}
