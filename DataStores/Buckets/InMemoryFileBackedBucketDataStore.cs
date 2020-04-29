using StoryBuckets.DataStores.FileStorage;
using StoryBuckets.DataStores.FileStore;
using StoryBuckets.DataStores.Generic;
using StoryBuckets.DataStores.Stories;
using StoryBuckets.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StoryBuckets.DataStores.Buckets
{
    public class InMemoryFileBackedBucketDataStore: InMemoryFileBackedDataStore<Bucket>
    {
        private readonly IFileBackedStoryDataStore _storyStore;

        public InMemoryFileBackedBucketDataStore(IStorageFolderProvider folderProvider, IFileBackedStoryDataStore storyStore)
            :base(folderProvider.GetStorageFolder<Bucket>("buckets"))
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

            foreach (var idBucketPair in Items)
            {
                var stories = await _storyStore.GetStoriesInBucket(idBucketPair.Key);
                foreach (var story in stories)
                {
                    idBucketPair.Value.Add(story);
                }
            }
        }
    }
}
