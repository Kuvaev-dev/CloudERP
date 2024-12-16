using System.ComponentModel.DataAnnotations;

namespace CloudERP.Models
{
    public class BranchMV
    {
        public int BranchID { get; set; }

        [Required]
        [StringLength(100)]
        public string BranchName { get; set; }

        [Required]
        [StringLength(50)]
        public string BranchContact { get; set; }

        [Required]
        [StringLength(200)]
        public string BranchAddress { get; set; }

        public int CompanyID { get; set; }
        public int? ParentBranchID { get; set; }
        public int BranchTypeID { get; set; }
        public string BranchTypeName { get; set; }
    }
}