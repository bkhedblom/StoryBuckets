using StoryBuckets.DataStores;
using StoryBuckets.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryBuckets.Services
{
    public class BucketService : IBucketService
    {
        private IDataStore<Bucket> _datastore;

        public BucketService(IDataStore<Bucket> datastore)
        {
            _datastore = datastore;
        }

        public async Task<IEnumerable<Bucket>> GetAllAsync()
        {
            await InitializeDataStoreIfNeeded();

            return await _datastore.GetAllAsync();
        }

        public async Task AddAsync(Bucket bucket)
        {
            await InitializeDataStoreIfNeeded();
            await _datastore.AddOrUpdateAsync(new[] { bucket });
        }

        private async Task InitializeDataStoreIfNeeded()
        {
            if (!_datastore.IsInitialized)
                await _datastore.InitializeAsync();
        }

        public async Task UpdateAsync(int id, Bucket bucket)
        {
            await InitializeDataStoreIfNeeded();
            await _datastore.UpdateAsync(id, bucket);
        }
    }
}
