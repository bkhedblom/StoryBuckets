using StoryBuckets.Client.Models;
using StoryBuckets.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoryBuckets.Client.ServerCommunication
{
    public class StorySync : IDataReader<SyncableStory>, IDataUpdater<SyncableStory>
    {
        private const string storyEndpoint = "stories";
        private IHttpClient _httpClient;

        public StorySync(IHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IReadOnlyCollection<SyncableStory>> ReadAsync()
        {
            var stories = await _httpClient.GetJsonAsync<Story[]>(storyEndpoint);
            return stories.Select(story => {
                var syncable = new SyncableStory(story);
                syncable.Updated += StoryUpdatedHandler;
                return syncable;
            }).ToList();
        }

        private async void StoryUpdatedHandler(object sender, EventArgs e)
            => await UpdateAsync(sender as SyncableStory);

        public async Task UpdateAsync(SyncableStory model)
            => _ = await _httpClient.PutJsonAsync("stories", model);
    }
}
