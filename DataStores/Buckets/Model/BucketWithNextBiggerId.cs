using StoryBuckets.Shared;
using StoryBuckets.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoryBuckets.DataStores.Buckets.Model
{
    class BucketWithNextBiggerId:Bucket
    {
        private readonly int? _nextBiggerBucketId;

        public BucketWithNextBiggerId(IEnumerable<Story> stories, int? nextBiggerBucketId) : base(stories) 
            => _nextBiggerBucketId = nextBiggerBucketId;

        public override IBucket NextBiggerBucket
        {
            get
            {
                if (base.NextBiggerBucket == null && _nextBiggerBucketId != null)
                    throw new InvalidOperationException($"NextBiggerBucket have not been set; should've been the bucket with id {_nextBiggerBucketId}");

                return base.NextBiggerBucket;
            }
            set
            {
                if (value.Id != _nextBiggerBucketId)
                    throw new InvalidOperationException($"NextBiggerBucket set to wrong Bucket! Expected Bucket with Id {_nextBiggerBucketId}");
                base.NextBiggerBucket = value;
            }
        }
    }
}
