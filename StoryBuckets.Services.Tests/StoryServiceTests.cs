using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StoryBuckets.DataStores;
using StoryBuckets.Integrations;
using StoryBuckets.Services;
using StoryBuckets.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryBuckets.Services.Tests
{
    [TestClass()]
    public class StoryServiceTests
    {
        [TestMethod()]
        public void GetAll_returns_some_stories()
        {
            //Arrange
            var dataStore = new Mock<IDataStore<Story>>();
            dataStore
                .Setup(fake => fake.GetAllAsync())
                .ReturnsAsync(new List<Story>());

            var integration = new Mock<IIntegration>();

            var service = new StoryService(dataStore.Object, integration.Object);

            //Act
            var result = service.GetAllAsync().Result;

            //Assert
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void GetAll_gets_stories_From_datastore()
        {
            //Arrange
            var dataStore = new Mock<IDataStore<Story>>();
            dataStore
                .Setup(fake => fake.GetAllAsync())
                .ReturnsAsync(new List<Story>());

            var integration = new Mock<IIntegration>();

            var service = new StoryService(dataStore.Object, integration.Object);

            //Act
            var result = service.GetAllAsync().Result;

            //Assert
            dataStore.Verify(mock => mock.GetAllAsync());
        }

        [TestMethod()]
        public void GetAll_returns_stories_from_datastore()
        {
            //Arrange
            var stories = new[]
            {
                new Mock<Story>().Object
            };
            var dataStore = new Mock<IDataStore<Story>>();
            dataStore
                .Setup(fake => fake.GetAllAsync())
                .ReturnsAsync(stories);

            var integration = new Mock<IIntegration>();

            var service = new StoryService(dataStore.Object, integration.Object);

            //Act
            var result = service.GetAllAsync().Result;

            //Assert
            Assert.AreEqual(stories.First(), result.First());
        }

        [TestMethod()]
        public void If_DataStore_is_empty_GetAll_use_Integration_to_fetch_stories()
        {
            //Arrange
            var dataStore = new Mock<IDataStore<Story>>();
            dataStore
                .SetupGet(fake => fake.IsEmpty)
                .Returns(true);

            var integration = new Mock<IIntegration>();

            var service = new StoryService(dataStore.Object, integration.Object);

            //Act
            var result = service.GetAllAsync().Result;

            //Assert
            integration.Verify(mock => mock.FetchAsync(), Times.Once);
        }

        [TestMethod()]
        public void After_fetching_stories_that_number_of_stories_are_added_to_the_datastore()
        {
            //Arrange
            var addedStories = new List<Story>();
            var dataStore = new Mock<IDataStore<Story>>();
            dataStore
                .SetupGet(fake => fake.IsEmpty)
                .Returns(false);
            dataStore
                .Setup(mock => mock.AddOrUpdateAsync(It.IsAny<IEnumerable<Story>>()))
                .Callback<IEnumerable<Story>>(items => addedStories.AddRange(items));

            var stories = new[]
            {
                new Mock<IStoryFromIntegration>().Object,
                new Mock<IStoryFromIntegration>().Object,
                new Mock<IStoryFromIntegration>().Object
            };
            var integration = new Mock<IIntegration>();
            integration
                .Setup(fake => fake.FetchAsync())
                .ReturnsAsync(stories);

            var service = new StoryService(dataStore.Object, integration.Object);

            //Act
            service.GetAllAsync().Wait();

            //Assert
            Assert.AreEqual(stories.Count(), addedStories.Count());
        }

        [TestMethod()]
        public void After_fetching_stories_Ids_are_mapped_to_stories_that_are_added_to_the_datastore()
        {
            //Arrange
            var addedStories = new List<Story>();
            var dataStore = new Mock<IDataStore<Story>>();
            dataStore
                .SetupGet(fake => fake.IsEmpty)
                .Returns(false);
            dataStore
                .Setup(mock => mock.AddOrUpdateAsync(It.IsAny<IEnumerable<Story>>()))
                .Callback<IEnumerable<Story>>(items => addedStories.AddRange(items));

            var story1 = new Mock<IStoryFromIntegration>();
            story1
                .SetupGet(fake => fake.Id)
                .Returns(1);
            var story2 = new Mock<IStoryFromIntegration>();
            story2
                .SetupGet(fake => fake.Id)
                .Returns(2);
            var story3 = new Mock<IStoryFromIntegration>();
            story3
                .SetupGet(fake => fake.Id)
                .Returns(3);
            var storiesFromIntegration = new[]
            {
                story1.Object,
                story2.Object,
                story3.Object
            };

            var integration = new Mock<IIntegration>();
            integration
                .Setup(fake => fake.FetchAsync())
                .ReturnsAsync(storiesFromIntegration);

            var service = new StoryService(dataStore.Object, integration.Object);

            //Act
            service.GetAllAsync().Wait();

            //Assert
            foreach (var integrationStory in storiesFromIntegration)
            {
                Assert.IsNotNull(addedStories.SingleOrDefault(story => story.Id == integrationStory.Id), $"Story Id {integrationStory.Id} was not added!");
            }
        }

        [TestMethod()]
        public void After_fetching_stories_Titles_are_mapped_to_stories_that_are_added_to_the_datastore()
        {
            //Arrange
            var addedStories = new List<Story>();
            var dataStore = new Mock<IDataStore<Story>>();
            dataStore
                .SetupGet(fake => fake.IsEmpty)
                .Returns(false);
            dataStore
                .Setup(mock => mock.AddOrUpdateAsync(It.IsAny<IEnumerable<Story>>()))
                .Callback<IEnumerable<Story>>(items => addedStories.AddRange(items));

            var story1 = new Mock<IStoryFromIntegration>();
            story1
                .SetupGet(fake => fake.Title)
                .Returns("Title 1");
            var story2 = new Mock<IStoryFromIntegration>();
            story2
                .SetupGet(fake => fake.Title)
                .Returns("Title 2");
            var story3 = new Mock<IStoryFromIntegration>();
            story3
                .SetupGet(fake => fake.Title)
                .Returns("Title 3");
            var storiesFromIntegration = new[]
            {
                story1.Object,
                story2.Object,
                story3.Object
            };

            var integration = new Mock<IIntegration>();
            integration
                .Setup(fake => fake.FetchAsync())
                .ReturnsAsync(storiesFromIntegration);

            var service = new StoryService(dataStore.Object, integration.Object);

            //Act
            service.GetAllAsync().Wait();

            //Assert
            foreach (var integrationStory in storiesFromIntegration)
            {
                Assert.IsNotNull(addedStories.SingleOrDefault(story => story.Title == integrationStory.Title), $"Story Title {integrationStory.Title} was not added!");
            }
        }

        [TestMethod()]
        public void If_DataStore_is_not_initialized_GetAll_initializes_it()
        {
            //Arrange
            var dataStore = new Mock<IDataStore<Story>>();
            dataStore
                .SetupGet(fake => fake.IsInitialized)
                .Returns(false);

            var integration = new Mock<IIntegration>();

            var service = new StoryService(dataStore.Object, integration.Object);

            //Act
            service.GetAllAsync().Wait();

            //Assert
            dataStore.Verify(mock => mock.InitializeAsync(), Times.Once);
        }

        [TestMethod()]
        public void UpdateAsync_sends_story_to_data_store()
        {
            //Arrange
            var dataStore = new Mock<IDataStore<Story>>();

            var integration = new Mock<IIntegration>();

            var service = new StoryService(dataStore.Object, integration.Object);

            var id = 42;
            var story = new Story();

            //Act
            service.UpdateAsync(id, story).Wait();

            //Assert
            dataStore.Verify(mock => mock.UpdateAsync(id, story));
        }

        [TestMethod()]
        public void If_DataStore_is_not_initialized_Update_initializes_it()
        {
            //Arrange
            var dataStore = new Mock<IDataStore<Story>>();
            dataStore
                .SetupGet(fake => fake.IsInitialized)
                .Returns(false);

            var integration = new Mock<IIntegration>();

            var service = new StoryService(dataStore.Object, integration.Object);

            //Act
            service.UpdateAsync(42, new Story()).Wait();

            //Assert
            dataStore.Verify(mock => mock.InitializeAsync(), Times.Once);
        }

        [TestMethod()]
        public void GetAll_fetches_stories_from_integration_even_if_datastore_is_not_empty()
        {
            //Arrange
            var dataStore = new Mock<IDataStore<Story>>();
            dataStore
                .SetupGet(fake => fake.IsEmpty)
                .Returns(false);

            var integration = new Mock<IIntegration>();

            var service = new StoryService(dataStore.Object, integration.Object);

            //Act
            var result = service.GetAllAsync().Result;

            //Assert
            integration.Verify(mock => mock.FetchAsync(), Times.Once);
        }

        [TestMethod()]
        public async Task Existing_stories_gets_to_keep_their_IsInBucket_status()
        {
            //Arrange
            var existingStoryInBucket = new Story
            {
                Id = 2718,
                IsInBucket = true
            };
            var existingStoryNotInBucket = new Story
            {
                Id = 314,
                IsInBucket = false
            };

            var addedOrUpdatedStories = new List<Story>();
            var dataStore = new Mock<IDataStore<Story>>();
            dataStore
                .SetupGet(fake => fake.IsEmpty)
                .Returns(false);
            dataStore
                .Setup(mock => mock.AddOrUpdateAsync(It.IsAny<IEnumerable<Story>>()))
                .Callback<IEnumerable<Story>>(items => addedOrUpdatedStories.AddRange(items));
            dataStore
                .Setup(fake => fake.GetAllAsync())
                .ReturnsAsync(new[] { existingStoryInBucket, existingStoryNotInBucket });

            var existingStoryInIntegration = new Mock<IStoryFromIntegration>();
            existingStoryInIntegration
                .SetupGet(fake => fake.Id)
                .Returns(existingStoryInBucket.Id);
            var existingStory2InIntegration = new Mock<IStoryFromIntegration>();
            existingStory2InIntegration
                .SetupGet(fake => fake.Id)
                .Returns(existingStoryNotInBucket.Id);

            var stories = new[]
            {
                existingStoryInIntegration.Object,
                existingStory2InIntegration.Object,
                new Mock<IStoryFromIntegration>().Object,
                new Mock<IStoryFromIntegration>().Object
            };
            var integration = new Mock<IIntegration>();
            integration
                .Setup(fake => fake.FetchAsync())
                .ReturnsAsync(stories);

            var service = new StoryService(dataStore.Object, integration.Object);

            //Act
            _ = await service.GetAllAsync();

            //Assert
            var updatedVersionOfStoryInBucket = addedOrUpdatedStories.Single(story => story.Id == existingStoryInBucket.Id);
            Assert.AreEqual(existingStoryInBucket.IsInBucket, updatedVersionOfStoryInBucket.IsInBucket);
            var updatedVersionOfStoryNotInBucket = addedOrUpdatedStories.Single(story => story.Id == existingStoryNotInBucket.Id);
            Assert.AreEqual(existingStoryNotInBucket.IsInBucket, updatedVersionOfStoryNotInBucket.IsInBucket);
        }

        [TestMethod()]
        public void GetAll_only_fetches_from_datastore_once_when_it_starts_empty()
        {
            //Arrange
            var dataStore = new Mock<IDataStore<Story>>();
            dataStore
                .Setup(fake => fake.GetAllAsync())
                .ReturnsAsync(new List<Story>());
            dataStore
                .SetupGet(fake => fake.IsEmpty)
                .Returns(true);

            var integration = new Mock<IIntegration>();

            var service = new StoryService(dataStore.Object, integration.Object);

            //Act
            var result = service.GetAllAsync().Result;

            //Assert
            dataStore.Verify(mock => mock.GetAllAsync(), Times.Once);
            dataStore.Verify(mock => mock.GetAsync(It.IsAny<IEnumerable<int>>()), Times.Never);
        }

        [TestMethod()]
        public async Task Existing_stories_gets_their_title_updated()
        {
            //Arrange
            var existingStory = new Story
            {
                Id = 2718,
                Title = "Bar bar bar"
            };
            var newTitle = "Foobar";
            var addedOrUpdatedStories = new List<Story>();
            var dataStore = new Mock<IDataStore<Story>>();
            dataStore
                .SetupGet(fake => fake.IsEmpty)
                .Returns(false);
            dataStore
                .Setup(mock => mock.AddOrUpdateAsync(It.IsAny<IEnumerable<Story>>()))
                .Callback<IEnumerable<Story>>(items => addedOrUpdatedStories.AddRange(items));
            dataStore
                .Setup(fake => fake.GetAllAsync())
                .ReturnsAsync(new[] { existingStory });

            var existingStoryInIntegration = new Mock<IStoryFromIntegration>();
            existingStoryInIntegration
                .SetupGet(fake => fake.Id)
                .Returns(existingStory.Id);
            existingStoryInIntegration
                .SetupGet(fake => fake.Title)
                .Returns(newTitle);

            var stories = new[]
            {
                existingStoryInIntegration.Object,
                new Mock<IStoryFromIntegration>().Object,
                new Mock<IStoryFromIntegration>().Object
            };
            var integration = new Mock<IIntegration>();
            integration
                .Setup(fake => fake.FetchAsync())
                .ReturnsAsync(stories);

            var service = new StoryService(dataStore.Object, integration.Object);

            //Act
            _ = await service.GetAllAsync();

            //Assert
            var updatedVersionOfStory = addedOrUpdatedStories.Single(story => story.Id == existingStory.Id);
            Assert.AreEqual(newTitle, updatedVersionOfStory.Title);
        }

        [TestMethod()]
        public async Task When_updating_the_DataStore_from_Integration_all_the_new_stories_gets_added()
        {
            //Arrange
            var existingStory = new Story
            {
                Id = 2718,
                Title = "Foobar"
            };
            var addedOrUpdatedStories = new List<Story>();
            var dataStore = new Mock<IDataStore<Story>>();
            dataStore
                .SetupGet(fake => fake.IsEmpty)
                .Returns(false);
            dataStore
                .Setup(mock => mock.AddOrUpdateAsync(It.IsAny<IEnumerable<Story>>()))
                .Callback<IEnumerable<Story>>(items => addedOrUpdatedStories.AddRange(items));
            dataStore
                .Setup(fake => fake.GetAllAsync())
                .ReturnsAsync(new[] { existingStory });

            var existingStoryInIntegration = new Mock<IStoryFromIntegration>();
            existingStoryInIntegration
                .SetupGet(fake => fake.Id)
                .Returns(existingStory.Id);
            existingStoryInIntegration
                .SetupGet(fake => fake.Title)
                .Returns(existingStory.Title);

            var stories = new[]
            {
                existingStoryInIntegration.Object,
                new Mock<IStoryFromIntegration>().Object,
                new Mock<IStoryFromIntegration>().Object
            };
            var integration = new Mock<IIntegration>();
            integration
                .Setup(fake => fake.FetchAsync())
                .ReturnsAsync(stories);

            var service = new StoryService(dataStore.Object, integration.Object);

            //Act
            _ = await service.GetAllAsync();

            //Assert
            Assert.AreEqual(stories.Count(), addedOrUpdatedStories.Count());
        }
    }
}