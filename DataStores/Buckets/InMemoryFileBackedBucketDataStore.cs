using StoryBuckets.DataStores.Buckets.Model;
using StoryBuckets.DataStores.FileStore;
using StoryBuckets.DataStores.Generic;
using StoryBuckets.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StoryBuckets.DataStores.Buckets
{
    public class InMemoryFileBackedBucketDataStore: InMemoryFileBackedDataStore<Bucket, FileStoredBucket>
    {
        private readonly IDataStore<Story> _storyStore;
        private Dictionary<Bucket, int?> _nextBiggerIdForBuckets;
        private static readonly SemaphoreSlim _nextBiggerIdDictionaryLock = new SemaphoreSlim(1);

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
            if(!_storyStore.IsInitialized)
                await _storyStore.InitializeAsync();

            await _nextBiggerIdDictionaryLock.WaitAsync();
        
            try
            {
                _nextBiggerIdForBuckets = new Dictionary<Bucket, int?>();
                await base.InitializeAsync();

                foreach (var bucket in Items.Values)
                {
                    var nextBiggerId = _nextBiggerIdForBuckets[bucket];
                    if (nextBiggerId.HasValue)
                        bucket.NextBiggerBucket = Items[nextBiggerId.Value];
                }

                _nextBiggerIdForBuckets = null;
            }
            finally
            {
                _nextBiggerIdDictionaryLock.Release();
            }        
        }

        protected override async Task<Bucket> ConvertStorageItemToData(FileStoredBucket storedItem)
        {
            if (_nextBiggerIdForBuckets == null)
                throw new NotSupportedException("This method is supposed to be called only during initialisation.");

            var stories = await _storyStore.GetAsync(storedItem.StoryIds);
            var dataBucket = storedItem.ToData(stories);            
            _nextBiggerIdForBuckets.Add(dataBucket, storedItem.NextBiggerBucketId);
            return dataBucket;
        }
    }
}
