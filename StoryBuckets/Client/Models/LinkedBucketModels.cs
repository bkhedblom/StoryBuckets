using StoryBuckets.Client.ServerCommunication;
using StoryBuckets.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoryBuckets.Client.Models
{
    public class LinkedBucketModels : LinkedBuckets<SyncableBucket>, ILinkedBucketModels
    {
        private IDataCreator<SyncableBucket> _bucketcreator;
        private HashSet<SyncableBucket> _buckets;

        public LinkedBucketModels(IDataCreator<SyncableBucket> bucketcreator, IReadOnlyCollection<SyncableBucket> buckets) : base(buckets)
        {
            _bucketcreator = bucketcreator;
            _buckets = new HashSet<SyncableBucket>(buckets);
        }

        public async Task CreateEmptyBiggerThan(SyncableBucket smallerBucket)
        {
            var newBucket = await _bucketcreator.CreateEmptyAsync();
            if (smallerBucket == null)
            {
                SmallestBucket = newBucket;
            }
            else
            {
                if (!_buckets.Contains(smallerBucket))
                    throw new InvalidOperationException("Supplied smaller bucket must exist");

                newBucket.NextBiggerBucket = smallerBucket.NextBiggerBucket;
                smallerBucket.NextBiggerBucket = newBucket;
            }
            _buckets.Add(newBucket);
        }
    }
}
