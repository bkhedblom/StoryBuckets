using StoryBuckets.Client.ServerCommunication;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StoryBuckets.Client.Models;
using StoryBuckets.Shared;
using System.Linq;

namespace StoryBuckets.Client.ServerCommunication.Tests
{
    [TestClass()]
    public class StorySyncTests
    {
        public void ReadAsync_fetches_and_returns_data_from_the_stories_endpoint()
        {
            //Arrange
            var stories = new[]
            {
                new Story()
            };
            var httpclient = new Mock<IHttpClient>();
            httpclient
                .Setup(mock => mock.GetJsonAsync<Story[]>(It.IsAny<string>()))
                .ReturnsAsync(stories);

            var reader = new StorySync(httpclient.Object);

            //Act
            var result = reader.ReadAsync().Result;

            //Assert
            httpclient.Verify(mock => mock.GetJsonAsync<Story[]>("stories"), Times.Once);
            Assert.AreEqual(stories.Single(), result.Single());
        }

        [TestMethod()]
        public void Puts_updated_story_to_the_stories_endpoint()
        {
            //Arrange
            var httpClient = new Mock<IHttpClient>();
            var updater = new StorySync(httpClient.Object);
            var story = new SyncableStory();

            //Act
            updater.UpdateAsync(story);

            //Assert
            httpClient.Verify(mock => mock.PutJsonAsync("stories", story));
        }

        [TestMethod()]
        public void Sets_event_listener_and_updates_Story_on_server_when_it_is_Updated()
        {
            //Arrange
            var httpClient = new Mock<IHttpClient>();
            httpClient
                .Setup(mock => mock.GetJsonAsync<Story[]>(It.IsAny<string>()))
                .ReturnsAsync(new[] { new Story() });
            var sync = new StorySync(httpClient.Object);

            var story = sync.ReadAsync().Result.Single();

            //Act
            story.IsInBucket = true;

            //Assert
            httpClient.Verify(mock => mock.PutJsonAsync("stories", story));
        }
    }
}