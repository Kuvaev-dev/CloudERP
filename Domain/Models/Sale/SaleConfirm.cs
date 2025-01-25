using System.ComponentModel.DataAnnotations;

namespace Domain.Models.FinancialModels
{
    public class SaleConfirm
    {
        [Required(ErrorMessage = "Customer ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Customer ID must be a positive integer.")]
        public int CustomerID { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; }
        public bool IsPayment { get; set; }
    }
}
