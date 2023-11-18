using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Models
{
    public class AccountLedgerModel
    {
        public int SNo { get; set; }
        public string Account { get; set; }
        public string Date { get; set; }
        public string Description { get; set; }
        public string Debit { get; set; }
        public string Credit { get; set; }
    }
}
