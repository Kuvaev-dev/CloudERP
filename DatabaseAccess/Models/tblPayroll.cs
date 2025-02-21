using System;
using System.Collections.Generic;

namespace DatabaseAccess.Models;

public partial class tblPayroll
{
    public int PayrollID { get; set; }

    public int EmployeeID { get; set; }

    public int BranchID { get; set; }

    public int CompanyID { get; set; }

    public double TransferAmount { get; set; }

    public string PayrollInvoiceNo { get; set; } = null!;

    public DateTime PaymentDate { get; set; }

    public string SalaryMonth { get; set; } = null!;

    public string SalaryYear { get; set; } = null!;

    public int UserID { get; set; }

    public virtual tblBranch Branch { get; set; } = null!;

    public virtual tblCompany Company { get; set; } = null!;

    public virtual tblEmployee Employee { get; set; } = null!;

    public virtual tblUser User { get; set; } = null!;
}
