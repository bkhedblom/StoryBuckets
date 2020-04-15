using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace StoryBuckets.Client.ServerCommunication
{
    public class StoryBucketsHttpClient:IHttpClient
    {
        private readonly HttpClient _client;

        public StoryBucketsHttpClient(HttpClient client)
        {
            _client = client;
        }

        public async Task<T> GetJsonAsync<T>(string endpoint)
            => await _client.GetJsonAsync<T>($"api/{endpoint}");
    }
}
