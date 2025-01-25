using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class Supplier
    {
        [Key]
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
        public int CompanyID { get; set; }
        public string CompanyName { get; set; }
        public int BranchID { get; set; }
        public string BranchName { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }
        public bool? IsActive { get; set; }
    }
}
