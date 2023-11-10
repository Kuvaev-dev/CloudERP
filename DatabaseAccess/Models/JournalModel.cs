using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Models
{
    public class JournalModel
    {
        public DateTime TransectionDate { get; set; }
        public string AccountSubControl { get; set; }
        public string TransectionTitle { get; set; }
        public int AccountSubControlID { get; set; }
        public string InvoiceNo { get; set; }
        public double Debit { get; set; }
        public double Credit { get; set; }
    }
}
