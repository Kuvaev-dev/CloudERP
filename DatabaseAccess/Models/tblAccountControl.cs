using System;
using System.Collections.Generic;

namespace DatabaseAccess.Models;

public partial class tblAccountControl
{
    public int AccountControlID { get; set; }

    public int CompanyID { get; set; }

    public int BranchID { get; set; }

    public int AccountHeadID { get; set; }

    public string AccountControlName { get; set; } = null!;

    public int UserID { get; set; }

    public bool? IsGlobal { get; set; }

    public virtual tblAccountHead AccountHead { get; set; } = null!;

    public virtual tblBranch Branch { get; set; } = null!;

    public virtual tblCompany Company { get; set; } = null!;

    public virtual tblUser User { get; set; } = null!;

    public virtual ICollection<tblAccountSetting> tblAccountSetting { get; set; } = new List<tblAccountSetting>();

    public virtual ICollection<tblAccountSubControl> tblAccountSubControl { get; set; } = new List<tblAccountSubControl>();

    public virtual ICollection<tblTransaction> tblTransaction { get; set; } = new List<tblTransaction>();
}
