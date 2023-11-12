using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Models
{
    public class TrialBalanceModel
    {
        public int FinancialYearID { get; set; }
        public string AccountSubControl { get; set; }
        public int AccountSubControlID { get; set; }
        public double Debit { get; set; }
        public double Credit { get; set; }
        public int BranchID { get; set; }
        public int CompanyID { get; set; }
    }
}
