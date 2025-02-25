using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class SaleConfirm
    {
        public int CustomerID { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }
        public bool IsPayment { get; set; }
        public int CompanyID { get; set; }
        public int BranchID { get; set; }
        public int UserID { get; set; }
    }
}
