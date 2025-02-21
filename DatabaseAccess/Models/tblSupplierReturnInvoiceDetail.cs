using System;
using System.Collections.Generic;

namespace DatabaseAccess.Models;

public partial class tblSupplierReturnInvoiceDetail
{
    public int SupplierReturnInvoiceDetailID { get; set; }

    public int SupplierInvoiceID { get; set; }

    public int SupplierInvoiceDetailID { get; set; }

    public int SupplierReturnInvoiceID { get; set; }

    public int ProductID { get; set; }

    public int PurchaseReturnQuantity { get; set; }

    public double PurchaseReturnUnitPrice { get; set; }

    public virtual tblStock Product { get; set; } = null!;

    public virtual tblSupplierInvoice SupplierInvoice { get; set; } = null!;

    public virtual tblSupplierInvoiceDetail SupplierInvoiceDetail { get; set; } = null!;

    public virtual tblSupplierReturnInvoice SupplierReturnInvoice { get; set; } = null!;
}
