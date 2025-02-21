using System;
using System.Collections.Generic;

namespace DatabaseAccess.Models;

public partial class tblSupplierPayment
{
    public int SupplierPaymentID { get; set; }

    public int SupplierID { get; set; }

    public int SupplierInvoiceID { get; set; }

    public int CompanyID { get; set; }

    public int BranchID { get; set; }

    public string InvoiceNo { get; set; } = null!;

    public double TotalAmount { get; set; }

    public double PaymentAmount { get; set; }

    public double RemainingBalance { get; set; }

    public int UserID { get; set; }

    public DateTime? InvoiceDate { get; set; }

    public virtual tblSupplier Supplier { get; set; } = null!;

    public virtual tblUser User { get; set; } = null!;
}
