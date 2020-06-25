using StoryBuckets.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoryBuckets.Client.ServerCommunication
{
    public class BucketSync : IDataReader<SyncableBucket>, IDataCreator<SyncableBucket>, IBucketReader
    {
        private const string Endpoint = "buckets";
        private IHttpClient _httpClient;

        public BucketSync(IHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<SyncableBucket> CreateEmptyAsync()
        {
            var newBucket = new SyncableBucket();
            await _httpClient.PostJsonAsync(Endpoint, newBucket);
            BindUpdatedEvent(newBucket);
            return newBucket;
        }

        public async Task<IReadOnlyCollection<SyncableBucket>> ReadAsync()
        {
            var buckets = await _httpClient.GetJsonAsync<SyncableBucket[]>(Endpoint);
            foreach (var bucket in buckets)
                BindUpdatedEvent(bucket);
            return buckets;
        }

        public async Task<ILinkedBucketModels> ReadLinkedBucketsAsync()
        {
            var buckets = await ReadAsync();
            return new LinkedBucketModels(this, buckets);
        }

        private void BindUpdatedEvent(SyncableBucket bucket)
            => bucket.Updated += async (sender, args) => await SyncBucketUpdateAsync((SyncableBucket)sender);

        private async Task SyncBucketUpdateAsync(SyncableBucket sender)
            => await _httpClient.PutJsonAsync(Endpoint, sender);
    }
}
