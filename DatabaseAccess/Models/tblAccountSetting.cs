using System;
using System.Collections.Generic;

namespace DatabaseAccess.Models;

public partial class tblAccountSetting
{
    public int AccountSettingID { get; set; }

    public int AccountHeadID { get; set; }

    public int AccountControlID { get; set; }

    public int AccountSubControlID { get; set; }

    public int AccountActivityID { get; set; }

    public int CompanyID { get; set; }

    public int BranchID { get; set; }

    public int? UserID { get; set; }

    public bool? IsGlobal { get; set; }

    public virtual tblAccountActivity AccountActivity { get; set; } = null!;

    public virtual tblAccountControl AccountControl { get; set; } = null!;

    public virtual tblAccountHead AccountHead { get; set; } = null!;

    public virtual tblAccountSubControl AccountSubControl { get; set; } = null!;

    public virtual tblBranch Branch { get; set; } = null!;

    public virtual tblCompany Company { get; set; } = null!;

    public virtual tblUser? User { get; set; }
}
