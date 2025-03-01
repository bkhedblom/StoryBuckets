﻿using Microsoft.AspNetCore.Components;
using StoryBuckets.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace StoryBuckets.Client.ServerCommunication
{
    public class StoryBucketsHttpClient:IHttpClient
    {
        private const string ApiPrefix = "/api/";
        private readonly HttpClient _client;

        public StoryBucketsHttpClient(HttpClient client)
        {
            _client = client;
        }

        public async Task<T> GetJsonAsync<T>(string endpoint)
            => await _client.GetJsonAsync<T>($"{ApiPrefix}{endpoint}");

        public async Task<T> PutJsonAsync<T>(string endpoint, T content) where T : IData
            => await _client.PutJsonAsync<T>($"{ApiPrefix}{endpoint}/{content.Id}", content);

        public async Task<T> PostJsonAsync<T>(string endpoint, T content)
            => await _client.PostJsonAsync<T>($"{ApiPrefix}{endpoint}", content);
    }
}
