using StoryBuckets.Client.ServerCommunication;
using StoryBuckets.Shared;
using StoryBuckets.Shared.Interfaces;

namespace StoryBuckets.Client.Models
{
    public interface IBucketModel:IBucket, ISyncable
    {
        void Add(Story story);
    }
}