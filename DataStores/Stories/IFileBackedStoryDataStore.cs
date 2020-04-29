using StoryBuckets.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StoryBuckets.DataStores.Stories
{
    public interface IFileBackedStoryDataStore:IInitializable
    {
        Task<IEnumerable<Story>> GetStoriesInBucket(int bucketId);
    }
}
