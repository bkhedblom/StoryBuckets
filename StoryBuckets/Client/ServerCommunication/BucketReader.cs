using StoryBuckets.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoryBuckets.Client.ServerCommunication
{
    public class BucketReader : IDataReader<IBucketModel>
    {
        private IHttpClient _httpClient;

        public BucketReader(IHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IReadOnlyCollection<IBucketModel>> ReadAsync() 
            => await _httpClient.GetJsonAsync<SyncableBucket[]>("buckets");
    }
}
