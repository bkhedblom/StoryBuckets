using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StoryBuckets.Client.Models;
using StoryBuckets.Client.ServerCommunication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryBuckets.Client.ServerCommunication.Tests
{
    [TestClass()]
    public class BucketSyncTests
    {
        [TestMethod]
        public void ReadAsync_fetches_and_returns_data_from_the_buckets_endpoint()
        {
            //Arrange
            var buckets = new[]
            {
                new SyncableBucket()
            };
            var httpclient = new Mock<IHttpClient>();
            httpclient
                .Setup(mock => mock.GetJsonAsync<SyncableBucket[]>(It.IsAny<string>()))
                .ReturnsAsync(buckets);

            var reader = new BucketSync(httpclient.Object);

            //Act
            var result = reader.ReadAsync().Result;

            //Assert
            httpclient.Verify(mock => mock.GetJsonAsync<SyncableBucket[]>("buckets"), Times.Once);
            Assert.AreEqual(buckets.Single(), result.Single());
        }

        [TestMethod]
        public async Task CreateEmpty_creates_and_Returns_a_new_bucket()
        {
            //Arrange
            var httpclient = new Mock<IHttpClient>();

            var syncer = new BucketSync(httpclient.Object);

            //Act
            var created = await syncer.CreateEmptyAsync();

            //Assert
            Assert.IsNotNull(created);
        }

        [TestMethod]
        public async Task CreateEmpty_posts_the_new_bucket_to_the_buckets_endpoint()
        {
            //Arrange
            var httpclient = new Mock<IHttpClient>();
            
            var syncer = new BucketSync(httpclient.Object);

            //Act
            var created = await syncer.CreateEmptyAsync();

            //Assert
            httpclient.Verify(mock => mock.PostJsonAsync("buckets", created), Times.Once);
        }
    }
}