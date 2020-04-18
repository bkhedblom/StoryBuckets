using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StoryBuckets.Shared;
using System.Linq;

namespace StoryBuckets.Client.ServerCommunication.Tests
{
    [TestClass()]
    public class StoryReaderTests
    {
        [TestMethod()]
        public void ReadAsync_fetches_and_returns_data_from_the_stories_endpoint()
        {
            //Arrange
            var stories = new[]
            {
                new Mock<Story>().Object
            };
            var httpclient = new Mock<IHttpClient>();
            httpclient
                .Setup(mock => mock.GetJsonAsync<Story[]>(It.IsAny<string>()))
                .ReturnsAsync(stories);

            var reader = new StoryReader(httpclient.Object);

            //Act
            var result = reader.ReadAsync().Result;

            //Assert
            httpclient.Verify(mock => mock.GetJsonAsync<Story[]>("stories"), Times.Once);
            Assert.AreEqual(stories.Single(), result.Single());
        }
    }
}