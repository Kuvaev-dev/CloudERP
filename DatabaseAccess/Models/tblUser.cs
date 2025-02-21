using System;
using System.Collections.Generic;

namespace DatabaseAccess.Models;

public partial class tblUser
{
    public int UserID { get; set; }

    public int UserTypeID { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string ContactNo { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? ResetPasswordCode { get; set; }

    public DateTime? LastPasswordResetRequest { get; set; }

    public DateTime? ResetPasswordExpiration { get; set; }

    public bool IsActive { get; set; }

    public string? Salt { get; set; }

    public virtual tblUserType UserType { get; set; } = null!;

    public virtual ICollection<tblAccountControl> tblAccountControl { get; set; } = new List<tblAccountControl>();

    public virtual ICollection<tblAccountHead> tblAccountHead { get; set; } = new List<tblAccountHead>();

    public virtual ICollection<tblAccountSetting> tblAccountSetting { get; set; } = new List<tblAccountSetting>();

    public virtual ICollection<tblAccountSubControl> tblAccountSubControl { get; set; } = new List<tblAccountSubControl>();

    public virtual ICollection<tblCategory> tblCategory { get; set; } = new List<tblCategory>();

    public virtual ICollection<tblCustomer> tblCustomer { get; set; } = new List<tblCustomer>();

    public virtual ICollection<tblCustomerInvoice> tblCustomerInvoice { get; set; } = new List<tblCustomerInvoice>();

    public virtual ICollection<tblCustomerPayment> tblCustomerPayment { get; set; } = new List<tblCustomerPayment>();

    public virtual ICollection<tblCustomerReturnInvoice> tblCustomerReturnInvoice { get; set; } = new List<tblCustomerReturnInvoice>();

    public virtual ICollection<tblCustomerReturnPayment> tblCustomerReturnPayment { get; set; } = new List<tblCustomerReturnPayment>();

    public virtual ICollection<tblFinancialYear> tblFinancialYear { get; set; } = new List<tblFinancialYear>();

    public virtual ICollection<tblPayroll> tblPayroll { get; set; } = new List<tblPayroll>();

    public virtual ICollection<tblPurchaseCart> tblPurchaseCart { get; set; } = new List<tblPurchaseCart>();

    public virtual ICollection<tblPurchaseCartDetail> tblPurchaseCartDetail { get; set; } = new List<tblPurchaseCartDetail>();

    public virtual ICollection<tblSaleCartDetail> tblSaleCartDetail { get; set; } = new List<tblSaleCartDetail>();

    public virtual ICollection<tblStock> tblStock { get; set; } = new List<tblStock>();

    public virtual ICollection<tblSupplier> tblSupplier { get; set; } = new List<tblSupplier>();

    public virtual ICollection<tblSupplierInvoice> tblSupplierInvoice { get; set; } = new List<tblSupplierInvoice>();

    public virtual ICollection<tblSupplierPayment> tblSupplierPayment { get; set; } = new List<tblSupplierPayment>();

    public virtual ICollection<tblSupplierReturnInvoice> tblSupplierReturnInvoice { get; set; } = new List<tblSupplierReturnInvoice>();

    public virtual ICollection<tblSupplierReturnPayment> tblSupplierReturnPayment { get; set; } = new List<tblSupplierReturnPayment>();

    public virtual ICollection<tblSupportTicket> tblSupportTicket { get; set; } = new List<tblSupportTicket>();

    public virtual ICollection<tblTask> tblTask { get; set; } = new List<tblTask>();

    public virtual ICollection<tblTransaction> tblTransaction { get; set; } = new List<tblTransaction>();
}
