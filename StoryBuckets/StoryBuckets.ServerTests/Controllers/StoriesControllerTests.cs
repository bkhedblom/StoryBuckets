using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StoryBuckets.Server.Controllers;
using StoryBuckets.Services;
using StoryBuckets.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StoryBuckets.Server.Controllers.Tests
{
    [TestClass()]
    public class StoriesControllerTests
    {
        [TestMethod()]
        public void Returns_data_From_Service_verbatim()
        {
            //Arrange
            var stories = new []
                {
                    new Story(),
                    new Story(),
                    new Story()
                };
            var service = new Mock<IStoryService>();
            service
                .Setup(fake => fake.GetAllAsync())
                .ReturnsAsync(stories);

            var controller = new StoriesController(service.Object);

            //Act
            var result = controller.Get().Result;

            //Assert
            service.Verify(mock => mock.GetAllAsync(), Times.Once);
            Assert.AreEqual(stories.Count(), result.Count());
            Assert.AreEqual(stories.First(), result.First());
        }

        [TestMethod()]
        public void Put_sends_data_straight_to_service()
        {
            //Arrange
            var service = new Mock<IStoryService>();

            var controller = new StoriesController(service.Object);
            
            var id = 42;
            var story = new Story();

            //Act
            controller.Put(id, story).Wait();

            //Assert
            service.Verify(mock => mock.UpdateAsync(id, story), Times.Once);
        }
    }
}