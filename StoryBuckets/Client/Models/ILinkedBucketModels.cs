﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace StoryBuckets.Client.Models
{
    public interface ILinkedBucketModels : IEnumerable<SyncableBucket>
    {
        Task CreateEmptyBiggerThan(SyncableBucket smallerBucket);
    }
}