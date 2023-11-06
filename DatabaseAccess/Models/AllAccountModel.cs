using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Models
{
    public class AllAccountModel
    {
        public int AccountHeadID { get; set; }
        public string AccountHeadName { get; set; }
        public int AccountControlID { get; set; }
        public string AccountControlName { get; set; }
        public int BranchID { get; set; }
        public int CompanyID { get; set; }
        public int AccountSubControlID { get; set; }
        public string AccountSubControl { get; set; }
    }
}
