using StoryBuckets.Shared.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StoryBuckets.Shared
{
    public class LinkedBuckets : LinkedBuckets<IBucket>, ILinkedBuckets
    {
        public LinkedBuckets():base() { }

        public LinkedBuckets(ICollection<IBucket> buckets) : base(buckets) { }
    }
}
