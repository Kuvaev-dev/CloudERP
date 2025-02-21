using System;
using System.Collections.Generic;

namespace DatabaseAccess.Models;

public partial class tblCustomerInvoiceDetail
{
    public int CustomerInvoiceDetailID { get; set; }

    public int CustomerInvoiceID { get; set; }

    public int ProductID { get; set; }

    public int SaleQuantity { get; set; }

    public double SaleUnitPrice { get; set; }

    public virtual tblCustomerInvoice CustomerInvoice { get; set; } = null!;

    public virtual tblStock Product { get; set; } = null!;

    public virtual ICollection<tblCustomerReturnInvoiceDetail> tblCustomerReturnInvoiceDetail { get; set; } = new List<tblCustomerReturnInvoiceDetail>();
}
