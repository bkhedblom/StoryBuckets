using StoryBuckets.Client.Models;
using StoryBuckets.Shared;
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
            var newServerBucket = await _httpClient.PostJsonAsync(Endpoint, new Bucket());
            var newBucket = new SyncableBucket(newServerBucket);
            BindUpdatedEvent(newBucket);
            return newBucket;
        }

        public async Task<IReadOnlyCollection<SyncableBucket>> ReadAsync()
        {
            var buckets = (await _httpClient.GetJsonAsync<Bucket[]>(Endpoint)).Select(b => new SyncableBucket(b)).ToList();
            foreach (var bucket in buckets)
            {
                if (bucket.NextBiggerBucketId != null)
                    bucket.NextBiggerBucket = buckets.Single(b => b.Id == bucket.NextBiggerBucketId);

                BindUpdatedEvent(bucket);
            }

            return buckets;
        }

        public async Task<ILinkedSyncableBuckets> ReadLinkedBucketsAsync()
        {
            var buckets = await ReadAsync();
            return new LinkedSyncableBuckets(this, buckets);
        }

        private void BindUpdatedEvent(SyncableBucket bucket)
            => bucket.Updated += async (sender, args) => await SyncBucketUpdateAsync((SyncableBucket)sender);

        private async Task SyncBucketUpdateAsync(Bucket sender)
            => await _httpClient.PutJsonAsync(Endpoint, sender);
    }
}
