using System.Collections.Generic;

namespace Domain.Models.FinancialModels
{
    public class SaleReturnConfirm
    {
        public List<int> ProductIDs { get; set; }
        public List<int> ReturnQty { get; set; }
        public int CustomerInvoiceID { get; set; }
        public bool IsPayment { get; set; }
    }
}
