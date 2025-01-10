using System.Collections.Generic;

namespace Domain.Models.FinancialModels
{
    public class PurchaseConfirm
    {
        public int SupplierId { get; set; }
        public string Description { get; set; }
        public bool IsPayment { get; set; }
        public IEnumerable<PurchaseCartDetail> PurchaseDetails { get; set; }
    }
}
