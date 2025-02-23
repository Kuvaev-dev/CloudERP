using System.ComponentModel.DataAnnotations;

namespace Domain.Models.FinancialModels
{
    public class SaleConfirm
    {
        public int CustomerID { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }
        public bool IsPayment { get; set; }
    }
}
