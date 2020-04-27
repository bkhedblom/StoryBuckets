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
            if (!_datastore.IsInitialized)
                await _datastore.InitializeAsync();

            return await _datastore.GetAllAsync();
        }
    }
}
