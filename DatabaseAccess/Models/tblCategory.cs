using System;
using System.Collections.Generic;

namespace DatabaseAccess.Models;

public partial class tblCategory
{
    public int CategoryID { get; set; }

    public string CategoryName { get; set; } = null!;

    public int BranchID { get; set; }

    public int CompanyID { get; set; }

    public int UserID { get; set; }

    public virtual tblBranch Branch { get; set; } = null!;

    public virtual tblCompany Company { get; set; } = null!;

    public virtual tblUser User { get; set; } = null!;

    public virtual ICollection<tblStock> tblStock { get; set; } = new List<tblStock>();
}
