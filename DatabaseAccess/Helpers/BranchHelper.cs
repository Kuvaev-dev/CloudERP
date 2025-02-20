using System.Collections.Generic;
using System.Linq;

namespace DatabaseAccess.Helpers
{
    public class BranchHelper
    {
        public List<int> GetBranchsIDs(int? brnchId, CloudDBEntities db)
        {
            if (!brnchId.HasValue) return new List<int>();

            var branchIDs = new List<int>();
            var queue = new Queue<int>();

            queue.Enqueue(brnchId.Value);

            while (queue.Count > 0)
            {
                int currentBranchId = queue.Dequeue();
                branchIDs.Add(currentBranchId);

                var subBranches = db.tblBranch
                    .Where(b => b.BrchID == currentBranchId)
                    .Select(b => b.BranchID)
                    .ToList();

                foreach (var sub in subBranches)
                {
                    queue.Enqueue(sub);
                }
            }

            return branchIDs;
        }
    }
}
