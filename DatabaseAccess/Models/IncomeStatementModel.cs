using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Models
{
    public class IncomeStatementModel
    {
        public string Title { get; set; }
        public double TotalAmount { get; set; }
        public AccountHeadTotal AccountHeadDetails { get; set; }
    }
}
