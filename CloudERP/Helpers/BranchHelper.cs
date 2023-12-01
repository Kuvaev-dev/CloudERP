using DatabaseAccess;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CloudERP.Helpers
{
    public class BranchHelper
    {
        public static List<int> GetBranchsIDs(int? brnchId, CloudDBEntities db)
        {
            List<int> branchIDs = new List<int>();
            List<int> isSubBranchsFirst = new List<int>();
            List<int> isSubBranchsSecond = new List<int>();

            int branchID = 0;
            branchID = Convert.ToInt32(brnchId);
            var brnch = db.tblBranch.Where(b => b.BrchID == branchID);

            foreach (var item in brnch)
            {
                isSubBranchsFirst.Add(item.BranchID);
            }

            while (isSubBranchsFirst.Count > 0)
            {
                foreach (var item in isSubBranchsFirst)
                {
                    branchIDs.Add(item);
                    foreach (var sub in db.tblBranch.Where(b => b.BrchID == item))
                    {
                        isSubBranchsSecond.Add(sub.BranchID);
                    }
                }
                isSubBranchsFirst.Clear();
                foreach (var item in isSubBranchsSecond)
                {
                    isSubBranchsFirst.Add(item);
                }
                isSubBranchsSecond.Clear();
            }

            return branchIDs;
        }
    }
}