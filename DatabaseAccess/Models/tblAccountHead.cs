using System;
using System.Collections.Generic;

namespace DatabaseAccess.Models;

public partial class tblAccountHead
{
    public int AccountHeadID { get; set; }

    public string AccountHeadName { get; set; } = null!;

    public int Code { get; set; }

    public int UserID { get; set; }

    public virtual tblUser User { get; set; } = null!;

    public virtual ICollection<tblAccountControl> tblAccountControl { get; set; } = new List<tblAccountControl>();

    public virtual ICollection<tblAccountSetting> tblAccountSetting { get; set; } = new List<tblAccountSetting>();

    public virtual ICollection<tblAccountSubControl> tblAccountSubControl { get; set; } = new List<tblAccountSubControl>();

    public virtual ICollection<tblTransaction> tblTransaction { get; set; } = new List<tblTransaction>();
}
