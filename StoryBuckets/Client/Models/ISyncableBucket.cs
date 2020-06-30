using StoryBuckets.Client.ServerCommunication;
using StoryBuckets.Shared;
using StoryBuckets.Shared.Interfaces;
using System.Collections.Generic;

namespace StoryBuckets.Client.Models
{
    public interface ISyncableBucket: ISyncable
    {
        void Add(Story story);
        IReadOnlyCollection<Story> Stories { get; }
        SyncableBucket NextBiggerBucket { get; set; }
    }
}