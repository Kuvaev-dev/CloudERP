using System;
using System.Collections.Generic;

namespace DatabaseAccess.Models;

public partial class tblTransaction
{
    public int TransactionID { get; set; }

    public int FinancialYearID { get; set; }

    public int AccountHeadID { get; set; }

    public int AccountControlID { get; set; }

    public int AccountSubControlID { get; set; }

    public string InvoiceNo { get; set; } = null!;

    public int CompanyID { get; set; }

    public int BranchID { get; set; }

    public double Credit { get; set; }

    public double Debit { get; set; }

    public DateTime TransectionDate { get; set; }

    public string TransectionTitle { get; set; } = null!;

    public int UserID { get; set; }

    public virtual tblAccountControl AccountControl { get; set; } = null!;

    public virtual tblAccountHead AccountHead { get; set; } = null!;

    public virtual tblAccountSubControl AccountSubControl { get; set; } = null!;

    public virtual tblFinancialYear FinancialYear { get; set; } = null!;

    public virtual tblUser User { get; set; } = null!;
}
