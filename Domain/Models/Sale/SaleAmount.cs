using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class SaleAmount
    {
        [Required(ErrorMessage = "Invoice ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Invoice ID must be a positive integer.")]
        public int InvoiceId { get; set; }

        [Range(0, float.MaxValue, ErrorMessage = "Previous Remaining Amount must be a positive value.")]
        public float PreviousRemainingAmount { get; set; }

        [Range(0, float.MaxValue, ErrorMessage = "Paid Amount must be a positive value.")]
        public float PaidAmount { get; set; }
        public int CompanyID { get; set; }
        public int BranchID { get; set; }
        public int UserID { get; set; }
    }
}
