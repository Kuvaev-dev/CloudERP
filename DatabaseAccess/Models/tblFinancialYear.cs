using System;
using System.Collections.Generic;

namespace DatabaseAccess.Models;

public partial class tblFinancialYear
{
    public int FinancialYearID { get; set; }

    public int UserID { get; set; }

    public string FinancialYear { get; set; } = null!;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public bool IsActive { get; set; }

    public virtual tblUser User { get; set; } = null!;

    public virtual ICollection<tblTransaction> tblTransaction { get; set; } = new List<tblTransaction>();
}
