using System;
using System.Collections.Generic;

namespace DatabaseAccess.Models;

public partial class tblCustomer
{
    public int CustomerID { get; set; }

    public string Customername { get; set; } = null!;

    public string CustomerContact { get; set; } = null!;

    public string CustomerAddress { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int BranchID { get; set; }

    public int CompanyID { get; set; }

    public int UserID { get; set; }

    public virtual tblBranch Branch { get; set; } = null!;

    public virtual tblCompany Company { get; set; } = null!;

    public virtual tblUser User { get; set; } = null!;

    public virtual ICollection<tblCustomerInvoice> tblCustomerInvoice { get; set; } = new List<tblCustomerInvoice>();

    public virtual ICollection<tblCustomerReturnInvoice> tblCustomerReturnInvoice { get; set; } = new List<tblCustomerReturnInvoice>();

    public virtual ICollection<tblCustomerReturnPayment> tblCustomerReturnPayment { get; set; } = new List<tblCustomerReturnPayment>();
}
