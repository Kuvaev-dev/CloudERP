using System;
using System.Collections.Generic;

namespace DatabaseAccess.Models;

public partial class tblCustomerReturnInvoiceDetail
{
    public int CustomerReturnInvoiceDetailID { get; set; }

    public int CustomerInvoiceDetailID { get; set; }

    public int CustomerInvoiceID { get; set; }

    public int CustomerReturnInvoiceID { get; set; }

    public int ProductID { get; set; }

    public int SaleReturnQuantity { get; set; }

    public double SaleReturnUnitPrice { get; set; }

    public virtual tblCustomerInvoice CustomerInvoice { get; set; } = null!;

    public virtual tblCustomerInvoiceDetail CustomerInvoiceDetail { get; set; } = null!;

    public virtual tblCustomerReturnInvoice CustomerReturnInvoice { get; set; } = null!;

    public virtual tblStock Product { get; set; } = null!;
}
