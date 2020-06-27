using StoryBuckets.Client.ServerCommunication;
using StoryBuckets.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoryBuckets.Client.Models
{
    public class LinkedSyncableBuckets : ILinkedSyncableBuckets
    {
        private IDataCreator<SyncableBucket> _bucketcreator;
        private HashSet<SyncableBucket> _buckets;
        private SyncableBucket _smallestBucket;

        public LinkedSyncableBuckets(IDataCreator<SyncableBucket> bucketcreator, IReadOnlyCollection<SyncableBucket> buckets)
        {
            if (buckets.Any() && buckets.Count(bucket => bucket.NextBiggerBucket == null) != 1)
                throw new InvalidOperationException("There must be one and only one bucket with no NextBiggerBucket set");

            foreach (var bucket in buckets)
            {
                int numberOfSmallerBuckets = buckets.Count(b => b.NextBiggerBucket == bucket);
                if (numberOfSmallerBuckets > 1)
                    throw new InvalidOperationException("The bucket hierarchy must be linear: no single bucket can be the NextBiggerBucket for more than a single other bucket");

                if (numberOfSmallerBuckets == 0)
                {
                    if (_smallestBucket != null)
                        throw new InvalidOperationException("There cannot be more than one smallest bucket!");

                    _smallestBucket = bucket;
                }

                if (bucket.NextBiggerBucket != null && !buckets.Contains(bucket.NextBiggerBucket))
                    throw new InvalidOperationException("All NextBiggerBucket must be contained in the collection");
            }

            _bucketcreator = bucketcreator;
            _buckets = new HashSet<SyncableBucket>(buckets);
        }

        public async Task CreateEmptyBiggerThan(ISyncableBucket smallerBucket)
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

        protected SyncableBucket SmallestBucket
        {
            get => _smallestBucket;

            set
            {
                value.NextBiggerBucket = _smallestBucket;
                _smallestBucket = value;
            }
        }

        public IEnumerator<SyncableBucket> GetEnumerator()
            => new BucketEnumerator(_smallestBucket);

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        private class BucketEnumerator : IEnumerator<SyncableBucket>
        {
            private SyncableBucket smallestBucket;
            private bool smallestBucketHasBeenRead = false;

            internal BucketEnumerator(SyncableBucket smallestBucket)
            {
                this.smallestBucket = smallestBucket;
            }

            public SyncableBucket Current { get; private set; }

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                //No additional resources to dispose
            }

            public bool MoveNext()
            {
                if (!smallestBucketHasBeenRead)
                {
                    Current = smallestBucket;
                    smallestBucketHasBeenRead = true;
                }
                else
                {
                    Current = Current?.NextBiggerBucket;
                }

                return Current != null;
            }

            public void Reset() => throw new NotSupportedException();
        }
    }
}
