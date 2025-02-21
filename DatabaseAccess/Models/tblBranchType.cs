using System;
using System.Collections.Generic;

namespace DatabaseAccess.Models;

public partial class tblBranchType
{
    public int BranchTypeID { get; set; }

    public string BranchType { get; set; } = null!;

    public virtual ICollection<tblBranch> tblBranch { get; set; } = new List<tblBranch>();
}
