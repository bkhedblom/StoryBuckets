using StoryBuckets.DataStores.Buckets.Model;
using StoryBuckets.DataStores.FileStore;
using StoryBuckets.DataStores.Generic;
using StoryBuckets.Shared;
using System;
using System.Collections.Generic;
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

        public override Task AddOrUpdateAsync(IEnumerable<Bucket> items)
            => throw new NotImplementedException();

        public override Task UpdateAsync(int id, Bucket item)
            => throw new NotImplementedException();

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
