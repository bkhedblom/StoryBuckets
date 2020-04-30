using StoryBuckets.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoryBuckets.Client.ServerCommunication
{
    public class BucketSync : IDataReader<IBucketModel>, IDataCreator<IBucketModel>
    {
        private const string Endpoint = "buckets";
        private IHttpClient _httpClient;

        public BucketSync(IHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IBucketModel> CreateEmptyAsync()
        {
            var newBucket = new SyncableBucket();
            await _httpClient.PostJsonAsync(Endpoint, newBucket);
            return newBucket;
        }

        public async Task<IReadOnlyCollection<IBucketModel>> ReadAsync() 
            => await _httpClient.GetJsonAsync<SyncableBucket[]>(Endpoint);
    }
}
