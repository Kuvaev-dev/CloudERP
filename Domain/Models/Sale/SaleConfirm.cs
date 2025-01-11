using System.ComponentModel.DataAnnotations;

namespace Domain.Models.FinancialModels
{
    public class SaleConfirm
    {
        [Required(ErrorMessage = "Customer ID is required.")]
        public int CustomerID { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Payment status (IsPayment) is required.")]
        public bool IsPayment { get; set; }
    }
}
