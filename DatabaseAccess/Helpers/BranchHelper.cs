using DatabaseAccess.Context;
using Domain.UtilsAccess;

namespace DatabaseAccess.Helpers
{
    public class BranchHelper : IBranchHelper
    {
        public List<int> GetBranchsIDs(int? brnchId, object db)
        {
            var context = db as CloudDBEntities ?? throw new ArgumentNullException(nameof(db));
            if (!brnchId.HasValue) return [];

            var branchIDs = new List<int>();
            var queue = new Queue<int>();

            queue.Enqueue(brnchId.Value);

            while (queue.Count > 0)
            {
                int currentBranchId = queue.Dequeue();
                branchIDs.Add(currentBranchId);

                var subBranches = context.tblBranch
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
