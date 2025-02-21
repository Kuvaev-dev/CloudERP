using System;
using System.Collections.Generic;

namespace DatabaseAccess.Models;

public partial class tblStock
{
    public int ProductID { get; set; }

    public int CategoryID { get; set; }

    public int CompanyID { get; set; }

    public int BranchID { get; set; }

    public string ProductName { get; set; } = null!;

    public int Quantity { get; set; }

    public double SaleUnitPrice { get; set; }

    public double CurrentPurchaseUnitPrice { get; set; }

    public DateTime ExpiryDate { get; set; }

    public DateTime Manufacture { get; set; }

    public int StockTreshHoldQuantity { get; set; }

    public string? Description { get; set; }

    public int UserID { get; set; }

    public bool? IsActive { get; set; }

    public virtual tblBranch Branch { get; set; } = null!;

    public virtual tblCategory Category { get; set; } = null!;

    public virtual tblCompany Company { get; set; } = null!;

    public virtual tblUser User { get; set; } = null!;

    public virtual ICollection<tblCustomerInvoiceDetail> tblCustomerInvoiceDetail { get; set; } = new List<tblCustomerInvoiceDetail>();

    public virtual ICollection<tblCustomerReturnInvoiceDetail> tblCustomerReturnInvoiceDetail { get; set; } = new List<tblCustomerReturnInvoiceDetail>();

    public virtual ICollection<tblPurchaseCartDetail> tblPurchaseCartDetail { get; set; } = new List<tblPurchaseCartDetail>();

    public virtual ICollection<tblSaleCartDetail> tblSaleCartDetail { get; set; } = new List<tblSaleCartDetail>();

    public virtual ICollection<tblSupplierInvoiceDetail> tblSupplierInvoiceDetail { get; set; } = new List<tblSupplierInvoiceDetail>();

    public virtual ICollection<tblSupplierReturnInvoiceDetail> tblSupplierReturnInvoiceDetail { get; set; } = new List<tblSupplierReturnInvoiceDetail>();
}
