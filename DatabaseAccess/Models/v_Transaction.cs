using System;
using System.Collections.Generic;

namespace DatabaseAccess.Models;

public partial class v_Transaction
{
    public int TransactionID { get; set; }

    public int FinancialYearID { get; set; }

    public int AccountHeadID { get; set; }

    public int AccountControlID { get; set; }

    public int AccountSubControlID { get; set; }

    public string AccountTitle { get; set; } = null!;

    public string InvoiceNo { get; set; } = null!;

    public int CompanyID { get; set; }

    public int BranchID { get; set; }

    public double Debit { get; set; }

    public double Credit { get; set; }

    public DateTime TransectionDate { get; set; }

    public string TransectionTitle { get; set; } = null!;

    public int UserID { get; set; }
}
