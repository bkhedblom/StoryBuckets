using StoryBuckets.Shared.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace StoryBuckets.Shared
{
    public class LinkedBuckets<T> : IEnumerable<T> where T:class,IBucket
    {
        private T _smallestBucket;

        public LinkedBuckets() : this(new List<T>()) { }

        public LinkedBuckets(IReadOnlyCollection<T> buckets)
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

        }


        protected T SmallestBucket
        {
            get => _smallestBucket; 
            
            set
            {
                value.NextBiggerBucket = _smallestBucket;
                _smallestBucket = value;
            }
        }

        public IEnumerator<T> GetEnumerator()
            => new BucketEnumerator(_smallestBucket);

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        private class BucketEnumerator : IEnumerator<T>
        {
            private T smallestBucket;
            private bool smallestBucketHasBeenRead = false;

            internal BucketEnumerator(T smallestBucket)
            {
                this.smallestBucket = smallestBucket;
            }

            public T Current { get; private set; }

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
                    Current = Current?.NextBiggerBucket as T;
                }

                return Current != null;
            }

            public void Reset() => throw new NotSupportedException();
        }
    }
}