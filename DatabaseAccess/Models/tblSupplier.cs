using System;
using System.Collections.Generic;

namespace DatabaseAccess.Models;

public partial class tblSupplier
{
    public int SupplierID { get; set; }

    public string SupplierName { get; set; } = null!;

    public string SupplierConatctNo { get; set; } = null!;

    public string? SupplierAddress { get; set; }

    public string? SupplierEmail { get; set; }

    public string? Discription { get; set; }

    public int BranchID { get; set; }

    public int CompanyID { get; set; }

    public int UserID { get; set; }

    public virtual tblBranch Branch { get; set; } = null!;

    public virtual tblCompany Company { get; set; } = null!;

    public virtual tblUser User { get; set; } = null!;

    public virtual ICollection<tblPurchaseCart> tblPurchaseCart { get; set; } = new List<tblPurchaseCart>();

    public virtual ICollection<tblSupplierInvoice> tblSupplierInvoice { get; set; } = new List<tblSupplierInvoice>();

    public virtual ICollection<tblSupplierPayment> tblSupplierPayment { get; set; } = new List<tblSupplierPayment>();

    public virtual ICollection<tblSupplierReturnInvoice> tblSupplierReturnInvoice { get; set; } = new List<tblSupplierReturnInvoice>();

    public virtual ICollection<tblSupplierReturnPayment> tblSupplierReturnPayment { get; set; } = new List<tblSupplierReturnPayment>();
}
