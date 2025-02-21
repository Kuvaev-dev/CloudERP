using System;
using System.Collections.Generic;

namespace DatabaseAccess.Models;

public partial class tblSupplierInvoiceDetail
{
    public int SupplierInvoiceDetailID { get; set; }

    public int SupplierInvoiceID { get; set; }

    public int ProductID { get; set; }

    public int PurchaseQuantity { get; set; }

    public double PurchaseUnitPrice { get; set; }

    public virtual tblStock Product { get; set; } = null!;

    public virtual tblSupplierInvoice SupplierInvoice { get; set; } = null!;

    public virtual ICollection<tblSupplierReturnInvoiceDetail> tblSupplierReturnInvoiceDetail { get; set; } = new List<tblSupplierReturnInvoiceDetail>();
}
