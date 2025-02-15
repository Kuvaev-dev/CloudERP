using System.ComponentModel.DataAnnotations;

namespace Domain.Models.FinancialModels
{
    public class PurchaseConfirm
    {
        [Required(ErrorMessage = "Supplier ID is required.")]
        public int SupplierId { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "IsPayment flag is required.")]
        public bool IsPayment { get; set; }
        public int CompanyID { get; set; }
        public int BranchID { get; set; }
        public int UserID { get; set; }
    }
}