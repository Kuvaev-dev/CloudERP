using System;
using System.Collections.Generic;

namespace DatabaseAccess.Models;

public partial class tblPurchaseCartDetail
{
    public int PurchaseCartDetailID { get; set; }

    public int ProductID { get; set; }

    public int PurchaseQuantity { get; set; }

    public double PurchaseUnitPrice { get; set; }

    public int CompanyID { get; set; }

    public int BranchID { get; set; }

    public int UserID { get; set; }

    public virtual tblBranch Branch { get; set; } = null!;

    public virtual tblCompany Company { get; set; } = null!;

    public virtual tblStock Product { get; set; } = null!;

    public virtual tblUser User { get; set; } = null!;
}
