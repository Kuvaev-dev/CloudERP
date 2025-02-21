using System;
using System.Collections.Generic;

namespace DatabaseAccess.Models;

public partial class tblCustomerPayment
{
    public int CustomerPaymentID { get; set; }

    public int CustomerID { get; set; }

    public int CustomerInvoiceID { get; set; }

    public int BranchID { get; set; }

    public int CompanyID { get; set; }

    public string InvoiceNo { get; set; } = null!;

    public double TotalAmount { get; set; }

    public double PaidAmount { get; set; }

    public double RemainingBalance { get; set; }

    public int UserID { get; set; }

    public DateTime? InvoiceDate { get; set; }

    public virtual tblBranch Branch { get; set; } = null!;

    public virtual tblCompany Company { get; set; } = null!;

    public virtual tblCustomerInvoice CustomerInvoice { get; set; } = null!;

    public virtual tblUser User { get; set; } = null!;
}
