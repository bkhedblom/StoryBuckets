using StoryBuckets.Server.DataStores;
using StoryBuckets.Shared;
using StoryBuckets.Shared.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoryBuckets.Server.Services
{
    public class StoryService : IStoryService
    {
        private readonly IDataStore<IStory> _dataStore;

        public StoryService(IDataStore<IStory> dataStore)
        {
            _dataStore = dataStore;
        }

        public async Task<IEnumerable<IStory>> GetAllAsync()
        {
           return await _dataStore.GetAllAsync();
        }
    }
}
