using StoryBuckets.Client.ServerSync;
using StoryBuckets.Shared;

namespace StoryBuckets.Client.Models
{
    public interface IBucketModel:IBucket, ISyncable
    {
        void Add(IStory story);
    }
}