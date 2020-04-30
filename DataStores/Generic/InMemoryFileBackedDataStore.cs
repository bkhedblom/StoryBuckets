using StoryBuckets.DataStores.FileStorage;
using StoryBuckets.DataStores.Generic.Model;
using StoryBuckets.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StoryBuckets.DataStores.Generic
{
    public class InMemoryFileBackedDataStore<TData, TStorage> : InMemoryDataStore<TData> 
        where TData : IData 
        where TStorage: IFileStoredData<TData>, new()
    {
        private readonly IStorageFolder<TStorage> _folder;
        private bool _initialized;

        public InMemoryFileBackedDataStore(IStorageFolder<TStorage> storageFolder)
        {
            _folder = storageFolder;
        }

        public override bool IsInitialized => _initialized;

        public override async Task AddOrUpdateAsync(IEnumerable<TData> items)
        {
            foreach (var item in items)
            {
                var storageItem = new TStorage();
                storageItem.MapFromData(item);
                if (IdIsInStore(item.Id))
                {
                    await _folder.ReplaceFileWithItemAsync(item.Id.ToString(), storageItem);
                }
                else
                {
                    await _folder.CreateFileForItemAsync(storageItem, item.Id.ToString());
                }
            }
            await base.AddOrUpdateAsync(items);
        }


        public override async Task InitializeAsync()
        {
            await foreach (var storedItem in _folder.GetStoredItemsAsync())
            {
                var dataItem = await ConvertStorageItemToData(storedItem);
                await AddToBaseAsync(dataItem);
            }
            _initialized = true;
        }

        protected virtual Task<TData> ConvertStorageItemToData(TStorage storedItem) => Task.FromResult(storedItem.ToData());

        private async Task AddToBaseAsync(TData item)
            => await base.AddOrUpdateAsync(new[] { item });

    }
}
