using StoryBuckets.Integrations;
using StoryBuckets.Server.DataStores;
using StoryBuckets.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoryBuckets.Server.Services
{
    public class StoryService : IStoryService
    {
        private readonly IDataStore<Story> _dataStore;
        private readonly IIntegration _integration;

        public StoryService(IDataStore<Story> dataStore, IIntegration integration)
        {
            _dataStore = dataStore;
            _integration = integration;
        }

        public async Task<IEnumerable<Story>> GetAllAsync()
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
            var storiesFromIntegration = await _integration.FetchAsync();
            var storiesToAdd = storiesFromIntegration.Select(integrationStory => new Story
            {
                Id = integrationStory.Id,
                Title = integrationStory.Title
            });
            await _dataStore.AddAsync(storiesToAdd);
        }
    }
}
