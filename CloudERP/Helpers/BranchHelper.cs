using CloudERP.Models;
using DatabaseAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
        subBranch:
            foreach (var item in isSubBranchsFirst)
            {
                branchIDs.Add(item);
                foreach (var sub in db.tblBranch.Where(b => b.BrchID == item))
                {
                    isSubBranchsSecond.Add(sub.BranchID);
                }
            }
            if (isSubBranchsSecond.Count > 0)
            {
                isSubBranchsFirst.Clear();
                foreach (var item in isSubBranchsSecond)
                {
                    isSubBranchsFirst.Add(item);
                }
                isSubBranchsSecond.Clear();
                goto subBranch;
            }

            return branchIDs;
        }
    }
}