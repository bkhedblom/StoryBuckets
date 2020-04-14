using StoryBuckets.Shared;
using StoryBuckets.Shared.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoryBuckets.Client.ServerCommunication
{
    public class StoryReader : IDataReader<IStory>
    {
        private const string storyEndpoint = "stories";
        private IHttpClient _httpClient;

        public StoryReader(IHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IReadOnlyCollection<IStory>> ReadAsync()
            => await _httpClient.GetJsonAsync<Story[]>(storyEndpoint);
    }
}
