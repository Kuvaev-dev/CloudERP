using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class Supplier
    {
        public int SupplierID { get; set; }

        [Required(ErrorMessage = "Supplier Name is required.")]
        [StringLength(100, ErrorMessage = "Supplier Name cannot be longer than 100 characters.")]
        public string SupplierName { get; set; }

        [Required(ErrorMessage = "Supplier Address is required.")]
        [StringLength(250, ErrorMessage = "Supplier Address cannot be longer than 250 characters.")]
        public string SupplierAddress { get; set; }

        [Required(ErrorMessage = "Supplier Contact Number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string SupplierConatctNo { get; set; }

        [Required(ErrorMessage = "Supplier Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string SupplierEmail { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters.")]
        public string Discription { get; set; }

        [Required(ErrorMessage = "Company ID is required.")]
        public int CompanyID { get; set; }
        [Required(ErrorMessage = "Company Name is required.")]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "Branch ID is required.")]
        public int BranchID { get; set; }
        [Required(ErrorMessage = "Branch Name is required.")]
        public string BranchName { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        public int UserID { get; set; }
        [Required(ErrorMessage = "Branch Name is required.")]
        public string UserName { get; set; }

        public bool? IsActive { get; set; }
    }
}
