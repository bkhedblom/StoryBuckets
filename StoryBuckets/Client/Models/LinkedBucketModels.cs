using StoryBuckets.Client.ServerCommunication;
using StoryBuckets.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoryBuckets.Client.Models
{
    public class LinkedBucketModels : LinkedBuckets<IBucketModel>, ILinkedBucketModels
    {
        private IDataCreator<IBucketModel> _bucketcreator;
        private HashSet<IBucketModel> _buckets;

        public LinkedBucketModels(IDataCreator<IBucketModel> bucketcreator, IReadOnlyCollection<IBucketModel> buckets) : base(buckets)
        {
            _bucketcreator = bucketcreator;
            _buckets = new HashSet<IBucketModel>(buckets);
        }

        public async Task CreateEmptyBiggerThan(IBucketModel smallerBucket)
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
