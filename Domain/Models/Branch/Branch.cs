using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class Branch
    {
        [Key]
        public int BranchID { get; set; }

        [Required(ErrorMessage = "Branch Code (BrchID) is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Branch Code (BrchID) must be a positive integer.")]
        public int BrchID { get; set; }

        [Required(ErrorMessage = "Branch Name is required.")]
        [StringLength(100, ErrorMessage = "Branch Name cannot exceed 100 characters.")]
        public string BranchName { get; set; }

        [StringLength(50, ErrorMessage = "Branch Contact cannot exceed 50 characters.")]
        public string BranchContact { get; set; }

        [StringLength(200, ErrorMessage = "Branch Address cannot exceed 200 characters.")]
        public string BranchAddress { get; set; }

        [Required(ErrorMessage = "Latitude is required.")]
        public double Latitude { get; set; }

        [Required(ErrorMessage = "Longitude is required.")]
        public double Longitude { get; set; }

        [Required(ErrorMessage = "Company ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Company ID must be a positive integer.")]
        public int CompanyID { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Parent Branch ID must be a positive integer.")]
        public int? ParentBranchID { get; set; }

        [Required(ErrorMessage = "Branch Type ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Branch Type ID must be a positive integer.")]
        public int BranchTypeID { get; set; }

        [StringLength(50, ErrorMessage = "Branch Type Name cannot exceed 50 characters.")]
        public string BranchTypeName { get; set; }
    }
}
