using StoryBuckets.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoryBuckets.Client.ServerCommunication
{
    public class StoryReader : IDataReader<Story>
    {
        private const string storyEndpoint = "stories";
        private IHttpClient _httpClient;

        public StoryReader(IHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IReadOnlyCollection<Story>> ReadAsync()
            => await _httpClient.GetJsonAsync<Story[]>(storyEndpoint);
    }
}
