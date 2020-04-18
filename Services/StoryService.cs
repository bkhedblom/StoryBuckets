using StoryBuckets.DataStores;
using StoryBuckets.Integrations;
using StoryBuckets.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoryBuckets.Services
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
            return await _dataStore.GetAllAsync();
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
