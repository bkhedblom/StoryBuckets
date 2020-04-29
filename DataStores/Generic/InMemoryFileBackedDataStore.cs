using StoryBuckets.DataStores.FileStorage;
using StoryBuckets.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StoryBuckets.DataStores.Generic
{
    public class InMemoryFileBackedDataStore<T> : InMemoryDataStore<T> where T : IData
    {
        private readonly IStorageFolder<T> _folder;
        private bool _initialized;

        public InMemoryFileBackedDataStore(IStorageFolder<T> storageFolder)
        {
            _folder = storageFolder;
        }

        public override bool IsInitialized => _initialized;

        public override async Task AddOrUpdateAsync(IEnumerable<T> items)
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


        public override async Task InitializeAsync()
        {
            await foreach (var story in _folder.GetStoredItemsAsync())
            {
                await AddToBaseAsync(story);
            }
            _initialized = true;
        }

        private async Task AddToBaseAsync(T item)
            => await base.AddOrUpdateAsync(new[] { item });

    }
}
