using System.ComponentModel.DataAnnotations;

namespace CloudERP.Models
{
    public class SupplierMV
    {
        public int SupplierID { get; set; }

        [Required(ErrorMessage = "Supplier Name is required.")]
        [StringLength(100, ErrorMessage = "Supplier Name cannot exceed 100 characters.")]
        public string SupplierName { get; set; }

        [StringLength(200, ErrorMessage = "Supplier Address cannot exceed 200 characters.")]
        public string SupplierAddress { get; set; }

        [Required(ErrorMessage = "Supplier Contact Number is required.")]
        [StringLength(15, ErrorMessage = "Supplier Contact Number cannot exceed 15 characters.")]
        public string SupplierConatctNo { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string SupplierEmail { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Discription { get; set; }

        public int CompanyID { get; set; }
        public int BranchID { get; set; }
        public int UserID { get; set; }

        public bool? IsActive { get; set; }
    }
}