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
            await InitializeDataStoreIfNeeded();
            if (_dataStore.IsEmpty)
                await FillDataStoreFromIntegration();
            else
                await UpdateDataStoreFromIntegration();
            
            return await _dataStore.GetAllAsync();
        }

        public async Task UpdateAsync(int id, Story story)
        {
            await InitializeDataStoreIfNeeded();
            await _dataStore.UpdateAsync(id, story);
        }

        private async Task InitializeDataStoreIfNeeded()
        {
            if (!_dataStore.IsInitialized)
                await _dataStore.InitializeAsync();
        }

        private async Task FillDataStoreFromIntegration()
        {
            var storiesFromIntegration = await _integration.FetchAsync();
            await AddStoriesFromIntegrationToDataStore(storiesFromIntegration);
        }

        private async Task AddStoriesFromIntegrationToDataStore(IEnumerable<IStoryFromIntegration> storiesFromIntegration)
        {
            var storiesToAdd = storiesFromIntegration.Select(integrationStory => new Story
            {
                Id = integrationStory.Id,
                Title = integrationStory.Title
            });
            await _dataStore.AddOrUpdateAsync(storiesToAdd);
        }

        private async Task UpdateDataStoreFromIntegration()
        {
            var fetchingFromIntegration = _integration.FetchAsync();
            var fetchingExisting = _dataStore.GetAllAsync();
            await Task.WhenAll(fetchingFromIntegration, fetchingExisting);

            var storiesFromIntegration = fetchingFromIntegration.Result;
            var idsInIntegration = storiesFromIntegration.Select(story => story.Id);

            var existingStories = fetchingExisting.Result.ToList();

            var storiesToUpdate = existingStories.Where(story => idsInIntegration.Contains(story.Id));            
            foreach (var story in storiesToUpdate)
            {
                var storyFromIntegration = storiesFromIntegration.Single(integrationstory => integrationstory.Id == story.Id);
                story.Title = storyFromIntegration.Title;
            }

            await _dataStore.AddOrUpdateAsync(storiesToUpdate);
            
            var existingIds = existingStories.Select(story => story.Id);
            var storiesToAdd = storiesFromIntegration.Where(integrationStory => 
                                        !existingIds.Contains(integrationStory.Id));
            await AddStoriesFromIntegrationToDataStore(storiesToAdd);
        }
    }
}
