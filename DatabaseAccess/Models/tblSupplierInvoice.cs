using System;
using System.Collections.Generic;

namespace DatabaseAccess.Models;

public partial class tblSupplierInvoice
{
    public int SupplierInvoiceID { get; set; }

    public int SupplierID { get; set; }

    public int CompanyID { get; set; }

    public int BranchID { get; set; }

    public string InvoiceNo { get; set; } = null!;

    public double TotalAmount { get; set; }

    public DateTime InvoiceDate { get; set; }

    public string Description { get; set; } = null!;

    public int UserID { get; set; }

    public virtual tblBranch Branch { get; set; } = null!;

    public virtual tblCompany Company { get; set; } = null!;

    public virtual tblSupplier Supplier { get; set; } = null!;

    public virtual tblUser User { get; set; } = null!;

    public virtual ICollection<tblSupplierInvoiceDetail> tblSupplierInvoiceDetail { get; set; } = new List<tblSupplierInvoiceDetail>();

    public virtual ICollection<tblSupplierReturnInvoice> tblSupplierReturnInvoice { get; set; } = new List<tblSupplierReturnInvoice>();

    public virtual ICollection<tblSupplierReturnInvoiceDetail> tblSupplierReturnInvoiceDetail { get; set; } = new List<tblSupplierReturnInvoiceDetail>();

    public virtual ICollection<tblSupplierReturnPayment> tblSupplierReturnPayment { get; set; } = new List<tblSupplierReturnPayment>();
}
