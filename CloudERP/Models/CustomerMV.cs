using System.ComponentModel.DataAnnotations;

namespace CloudERP.Models
{
    public class CustomerMV
    {
        public int CustomerID { get; set; }

        [Required(ErrorMessage = "Customer name is required.")]
        [StringLength(100, ErrorMessage = "Customer name cannot be longer than 100 characters.")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Customer contact is required.")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        public string CustomerContact { get; set; }

        [Required(ErrorMessage = "Customer address is required.")]
        public string CustomerAddress { get; set; }

        public string Description { get; set; }

        public int BranchID { get; set; }
        public int CompanyID { get; set; }
        public int UserID { get; set; }
    }
}