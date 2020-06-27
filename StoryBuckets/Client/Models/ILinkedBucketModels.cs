using System.Collections.Generic;
using System.Threading.Tasks;

namespace StoryBuckets.Client.Models
{
    public interface ILinkedSyncableBuckets : IEnumerable<SyncableBucket>
    {
        Task CreateEmptyBiggerThan(ISyncableBucket smallerBucket);
    }
}