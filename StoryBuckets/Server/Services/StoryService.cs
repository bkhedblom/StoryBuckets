using StoryBuckets.Server.DataStores;
using StoryBuckets.Server.Integrations;
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
        private readonly IIntegration _integration;

        public StoryService(IDataStore<IStory> dataStore, IIntegration integration)
        {
            _dataStore = dataStore;
            _integration = integration;
        }

        public async Task<IEnumerable<IStory>> GetAllAsync()
        {
            if (_dataStore.IsEmpty)
            {
                await FillDataStoreFromIntegration();
            }
            var data = await _dataStore.GetAllAsync();
            return data;
        }

        private async Task FillDataStoreFromIntegration()
        {
            IEnumerable<IStory> storiesFromIntegration = await _integration.FetchAsync();
            await _dataStore.AddAsync(storiesFromIntegration);
        }
    }
}
