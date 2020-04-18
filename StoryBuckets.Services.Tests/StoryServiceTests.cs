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
            dataStore.Verify(mock => mock.GetAllAsync(), Times.Once);
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
        public void If_DataStore_is_not_empty_Integration_are_not_called()
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
            integration.Verify(mock => mock.FetchAsync(), Times.Never);
        }

        [TestMethod()]
        public void After_fetching_stories_that_number_of_stories_are_added_to_the_datastore()
        {
            //Arrange
            IEnumerable<Story> addedStories = Enumerable.Empty<Story>();
            var dataStore = new Mock<IDataStore<Story>>();
            dataStore
                .SetupGet(fake => fake.IsEmpty)
                .Returns(true);
            dataStore
                .Setup(mock => mock.AddAsync(It.IsAny<IEnumerable<Story>>()))
                .Callback<IEnumerable<Story>>(items => addedStories = items);

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
            IEnumerable<Story> addedStories = Enumerable.Empty<Story>();
            var dataStore = new Mock<IDataStore<Story>>();
            dataStore
                .SetupGet(fake => fake.IsEmpty)
                .Returns(true);
            dataStore
                .Setup(mock => mock.AddAsync(It.IsAny<IEnumerable<Story>>()))
                .Callback<IEnumerable<Story>>(items => addedStories = items);

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
            IEnumerable<Story> addedStories = Enumerable.Empty<Story>();
            var dataStore = new Mock<IDataStore<Story>>();
            dataStore
                .SetupGet(fake => fake.IsEmpty)
                .Returns(true);
            dataStore
                .Setup(mock => mock.AddAsync(It.IsAny<IEnumerable<Story>>()))
                .Callback<IEnumerable<Story>>(items => addedStories = items);

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
    }
}