using StoryBuckets.DataStores.FileStorage;
using StoryBuckets.DataStores.FileStore;
using StoryBuckets.DataStores.Generic;
using StoryBuckets.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StoryBuckets.DataStores.Stories
{
    public class InMemoryFileBackedStoryDataStore : InMemoryFileBackedDataStore<Story>, IFileBackedStoryDataStore
    {

        public InMemoryFileBackedStoryDataStore(IStorageFolderProvider fileStore) : base(fileStore.GetStorageFolder<Story>("stories"))
        {
        }

        public Task<IEnumerable<Story>> GetStoriesInBucket(int bucketId)
        {
            throw new NotImplementedException();
        }
    }
}
