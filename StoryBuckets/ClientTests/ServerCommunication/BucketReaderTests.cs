using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StoryBuckets.Client.Models;
using StoryBuckets.Client.ServerCommunication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StoryBuckets.Client.ServerCommunication.Tests
{
    [TestClass()]
    public class BucketReaderTests
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

            var reader = new BucketReader(httpclient.Object);

            //Act
            var result = reader.ReadAsync().Result;

            //Assert
            httpclient.Verify(mock => mock.GetJsonAsync<SyncableBucket[]>("buckets"), Times.Once);
            Assert.AreEqual(buckets.Single(), result.Single());
        }
    }
}