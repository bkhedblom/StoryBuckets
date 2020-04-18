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
    public class InMemoryFileBackedStoryDataStore : InMemoryDataStore<Story>
    {
        private readonly IStorageFolder<Story> _folder;
        private bool _isInitialized;

        public InMemoryFileBackedStoryDataStore(IStorageFolderProvider fileStore) : base()
        {
            _folder = fileStore.GetStorageFolder<Story>("stories");
        }

        public override bool IsInitialized => _isInitialized;
        public override async Task InitializeAsync()
        {
            await foreach (var story in _folder.GetStoredItemsAsync())
            {
                await AddToBaseAsync(story);
            }
            _isInitialized = true;
        }

        public override async Task AddOrUpdateAsync(IEnumerable<Story> items)
        {
            foreach (var item in items)
            {
                if (IdIsInStore(item.Id))
                {
                    await _folder.ReplaceFileWithItemAsync(item.Id.ToString(), item);
                }
                else
                {
                    await _folder.CreateFileForItemAsync(item, item.Id.ToString());
                }
            }
            await base.AddOrUpdateAsync(items);
        }

        private async Task AddToBaseAsync(Story item)
            => await base.AddOrUpdateAsync(new[] { item });
    }
}
