using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StoryBuckets.Server.DataStores;
using StoryBuckets.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StoryBuckets.Server.Services.Tests
{
    [TestClass()]
    public class StoryServiceTests
    {
        [TestMethod()]
        public void GetAll_returns_some_stories()
        {
            //Arrange
            var dataStore = new Mock<IDataStore<IStory>>();
            dataStore
                .Setup(fake => fake.GetAllAsync())
                .ReturnsAsync(new List<IStory>());

            var service = new StoryService(dataStore.Object);

            //Act
            var result = service.GetAllAsync().Result;

            //Assert
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void GetAll_gets_stories_From_datastore()
        {
            //Arrange
            var dataStore = new Mock<IDataStore<IStory>>();
            dataStore
                .Setup(fake => fake.GetAllAsync())
                .ReturnsAsync(new List<IStory>());

            var service = new StoryService(dataStore.Object);

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
                new Mock<IStory>().Object
            };
            var dataStore = new Mock<IDataStore<IStory>>();
            dataStore
                .Setup(fake => fake.GetAllAsync())
                .ReturnsAsync(stories);

            var service = new StoryService(dataStore.Object);

            //Act
            var result = service.GetAllAsync().Result;

            //Assert
            Assert.AreEqual(stories.First(), result.First());
        }

    }
}