using System;
using System.Collections.Generic;

namespace DatabaseAccess.Models;

public partial class tblSupplierReturnInvoice
{
    public int SupplierReturnInvoiceID { get; set; }

    public int SupplierInvoiceID { get; set; }

    public int SupplierID { get; set; }

    public int CompanyID { get; set; }

    public int BranchID { get; set; }

    public string InvoiceNo { get; set; } = null!;

    public double TotalAmount { get; set; }

    public DateTime InvoiceDate { get; set; }

    public string? Description { get; set; }

    public int UserID { get; set; }

    public virtual tblBranch Branch { get; set; } = null!;

    public virtual tblCompany Company { get; set; } = null!;

    public virtual tblSupplier Supplier { get; set; } = null!;

    public virtual tblSupplierInvoice SupplierInvoice { get; set; } = null!;

    public virtual tblUser User { get; set; } = null!;

    public virtual ICollection<tblSupplierReturnInvoiceDetail> tblSupplierReturnInvoiceDetail { get; set; } = new List<tblSupplierReturnInvoiceDetail>();

    public virtual ICollection<tblSupplierReturnPayment> tblSupplierReturnPayment { get; set; } = new List<tblSupplierReturnPayment>();
}
