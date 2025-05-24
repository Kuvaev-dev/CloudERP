using DatabaseAccess.Context;
using Domain.UtilsAccess;

namespace DatabaseAccess.Helpers
{
    public class BranchHelper : IBranchHelper
    {
        private readonly CloudDBEntities _dbContext;

        public BranchHelper(CloudDBEntities dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public List<int> GetBranchsIDs(int? brnchId)
        {
            if (!brnchId.HasValue) return [];

            var branchIDs = new List<int>();
            var queue = new Queue<int>();

            queue.Enqueue(brnchId.Value);

            while (queue.Count > 0)
            {
                int currentBranchId = queue.Dequeue();
                branchIDs.Add(currentBranchId);

                var subBranches = _dbContext.tblBranch
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
