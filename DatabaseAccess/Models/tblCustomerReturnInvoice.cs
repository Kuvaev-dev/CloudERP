using System;
using System.Collections.Generic;

namespace DatabaseAccess.Models;

public partial class tblCustomerReturnInvoice
{
    public int CustomerReturnInvoiceID { get; set; }

    public int CustomerInvoiceID { get; set; }

    public int CustomerID { get; set; }

    public int CompanyID { get; set; }

    public int BranchID { get; set; }

    public string InvoiceNo { get; set; } = null!;

    public double TotalAmount { get; set; }

    public DateTime InvoiceDate { get; set; }

    public string Description { get; set; } = null!;

    public int UserID { get; set; }

    public virtual tblBranch Branch { get; set; } = null!;

    public virtual tblCompany Company { get; set; } = null!;

    public virtual tblCustomer Customer { get; set; } = null!;

    public virtual tblCustomerInvoice CustomerInvoice { get; set; } = null!;

    public virtual tblUser User { get; set; } = null!;

    public virtual ICollection<tblCustomerReturnInvoiceDetail> tblCustomerReturnInvoiceDetail { get; set; } = new List<tblCustomerReturnInvoiceDetail>();

    public virtual ICollection<tblCustomerReturnPayment> tblCustomerReturnPayment { get; set; } = new List<tblCustomerReturnPayment>();
}
