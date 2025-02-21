using System;
using System.Collections.Generic;

namespace DatabaseAccess.Models;

public partial class tblAccountSubControl
{
    public int AccountSubControlID { get; set; }

    public int AccountHeadID { get; set; }

    public int AccountControlID { get; set; }

    public int CompanyID { get; set; }

    public int BranchID { get; set; }

    public string AccountSubControlName { get; set; } = null!;

    public int UserID { get; set; }

    public bool? IsGlobal { get; set; }

    public virtual tblAccountControl AccountControl { get; set; } = null!;

    public virtual tblAccountHead AccountHead { get; set; } = null!;

    public virtual tblBranch Branch { get; set; } = null!;

    public virtual tblUser User { get; set; } = null!;

    public virtual ICollection<tblAccountSetting> tblAccountSetting { get; set; } = new List<tblAccountSetting>();

    public virtual ICollection<tblTransaction> tblTransaction { get; set; } = new List<tblTransaction>();
}
