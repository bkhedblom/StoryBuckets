using StoryBuckets.Client.ServerCommunication;
using StoryBuckets.Shared;
using StoryBuckets.Shared.Interfaces;
using System.Collections.Generic;

namespace StoryBuckets.Client.Models
{
    public interface IBucketModel: ISyncable
    {
        void Add(Story story);
        IReadOnlyCollection<Story> Stories { get; }
        Bucket NextBiggerBucket { get; set; }
    }
}