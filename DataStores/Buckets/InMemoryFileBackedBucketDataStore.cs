using StoryBuckets.DataStores.Buckets.Model;
using StoryBuckets.DataStores.FileStore;
using StoryBuckets.DataStores.Generic;
using StoryBuckets.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoryBuckets.DataStores.Buckets
{
    public class InMemoryFileBackedBucketDataStore: InMemoryFileBackedDataStore<Bucket, FileStoredBucket>
    {
        private readonly IDataStore<Story> _storyStore;

        public InMemoryFileBackedBucketDataStore(IStorageFolderProvider folderProvider, IDataStore<Story> storyStore)
            :base(folderProvider.GetStorageFolder<FileStoredBucket>("buckets"))
        {
            _storyStore = storyStore;
        }

        public override async Task AddOrUpdateAsync(IEnumerable<Bucket> items)
        {
            var stories = items.SelectMany(bucket => bucket.Stories);
            var addingStories = _storyStore.AddOrUpdateAsync(stories);
            var addingItems = base.AddOrUpdateAsync(items);
            await Task.WhenAll(addingStories, addingItems);
        }        

        public override async Task InitializeAsync()
        {
            var initializations = new List<Task>();
            
            if(!_storyStore.IsInitialized)
                initializations.Add(_storyStore.InitializeAsync());

            initializations.Add(base.InitializeAsync());
            await Task.WhenAll(initializations);
        }

        protected override async Task<Bucket> ConvertStorageItemToData(FileStoredBucket storedItem)
        {
            var stories = await _storyStore.GetAsync(storedItem.StoryIds);
            return storedItem.ToData(stories);
        }
    }
}
