using System;
using System.Collections.Generic;

namespace DatabaseAccess.Models;

public partial class tblPurchaseCart
{
    public int PurchaseCartID { get; set; }

    public int SupplierID { get; set; }

    public int CompanyID { get; set; }

    public int BranchID { get; set; }

    public int InvoiceNo { get; set; }

    public double TotalAmount { get; set; }

    public DateTime InvoiceDate { get; set; }

    public string Description { get; set; } = null!;

    public int UserID { get; set; }

    public virtual tblBranch Branch { get; set; } = null!;

    public virtual tblCompany Company { get; set; } = null!;

    public virtual tblSupplier Supplier { get; set; } = null!;

    public virtual tblUser User { get; set; } = null!;
}
