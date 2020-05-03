using StoryBuckets.DataStores.FileStorage;
using StoryBuckets.DataStores.FileStore;
using StoryBuckets.DataStores.Generic;
using StoryBuckets.DataStores.Stories.Model;
using StoryBuckets.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StoryBuckets.DataStores.Stories
{
    public class InMemoryFileBackedStoryDataStore : InMemoryFileBackedDataStore<Story, FileStoredStory>
    {

        public InMemoryFileBackedStoryDataStore(IStorageFolderProvider fileStore) : base(fileStore.GetStorageFolder<FileStoredStory>("stories"))
        {
        }

    }
}