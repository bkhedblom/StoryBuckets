using StoryBuckets.DataStores.FileStorage;
using StoryBuckets.DataStores.Generic.Model;
using StoryBuckets.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StoryBuckets.DataStores.Generic
{
    public abstract class InMemoryFileBackedDataStore<TData, TStorage> : InMemoryDataStore<TData> 
        where TData : IData 
        where TStorage: IFileStoredData<TData>, new()
    {
        private static readonly SemaphoreSlim _fileProcessLock = new SemaphoreSlim(1);
        private readonly IStorageFolder<TStorage> _folder;
        private bool _initialized;

        protected InMemoryFileBackedDataStore(IStorageFolder<TStorage> storageFolder)
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
                await _fileProcessLock.WaitAsync();
                try
                {
                    if (item.Id != 0 && IdIsInStore(item.Id))
                    {
                        await _folder.ReplaceFileWithItemAsync(item.Id.ToString(), storageItem);
                    }
                    else
                    {
                        if (item.Id == 0)
                            item.Id = GetNextId();

                        await _folder.CreateFileForItemAsync(storageItem, item.Id.ToString());
                    }
                }
                finally
                {
                    _fileProcessLock.Release();
                }
            }
            await base.AddOrUpdateAsync(items);
        }

        public override async Task InitializeAsync()
        {
            await _fileProcessLock.WaitAsync();
            try
            {
                await foreach (var storedItem in _folder.GetStoredItemsAsync())
                {
                    var dataItem = await ConvertStorageItemToData(storedItem);
                    await AddToBaseAsync(dataItem);
                }
                _initialized = true;
            }
            finally
            {
                _fileProcessLock.Release();
            }           
        }

        protected virtual Task<TData> ConvertStorageItemToData(TStorage storedItem) => Task.FromResult(storedItem.ToData());

        private async Task AddToBaseAsync(TData item)
            => await base.AddOrUpdateAsync(new[] { item });
    }
}
