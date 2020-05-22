﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StoryBuckets.Client.Models;
using StoryBuckets.Client.ServerCommunication;
using StoryBuckets.Shared;
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
        private const string BUCKETS = "buckets";

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
            httpclient.Verify(mock => mock.GetJsonAsync<SyncableBucket[]>(BUCKETS), Times.Once);
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
            httpclient.Verify(mock => mock.PostJsonAsync(BUCKETS, created), Times.Once);
        }

        [TestMethod()]
        public async Task ReadAsync_buckets_are_PUT_to_the_server_when_updated()
        {
            //Arrange
            var id1 = 2718;
            var id2 = 31415;
            var buckets = new[] 
            { 
                new SyncableBucket { Id = id1 }, 
                new SyncableBucket { Id = id2 } 
            };

            var updatedIds = new List<int>();

            var httpclient = new Mock<IHttpClient>();
            httpclient
                .Setup(fake => fake.GetJsonAsync<SyncableBucket[]>(It.IsAny<string>()))
                .ReturnsAsync(buckets);
            httpclient
                .Setup(mock => mock.PutJsonAsync<SyncableBucket>(It.IsAny<string>(), It.IsAny<SyncableBucket>()))
                .Callback<string, SyncableBucket>((_, updatedBucket) => updatedIds.Add(updatedBucket.Id));


            var reader = new BucketSync(httpclient.Object);
            var fetchedBuckets = await reader.ReadAsync();
            Assert.AreEqual(buckets.Count(), fetchedBuckets.Count(), "Test aborted because the reading precondition did not work as expected");

            //Act
            foreach (var bucket in fetchedBuckets)
            {
                bucket.Add(new Story());
            }

            //Assert
            httpclient.Verify(mock => mock.PutJsonAsync(BUCKETS, It.IsAny<SyncableBucket>()), Times.Exactly(buckets.Count()));
            foreach (var bucket in buckets)
            {
                Assert.IsTrue(updatedIds.Contains(bucket.Id), $"Bucket Id {bucket.Id} was not updated!");
            }
        }

        [TestMethod()]
        public async Task Newly_Created_Bucket_are_PUT_to_the_server_when_updated()
        {
            //Arrange
            var updatedId = -1;

            var httpclient = new Mock<IHttpClient>();
            httpclient
                .Setup(mock => mock.PutJsonAsync<SyncableBucket>(It.IsAny<string>(), It.IsAny<SyncableBucket>()))
                .Callback<string, SyncableBucket>((_, updatedBucket) => updatedId = updatedBucket.Id);


            var reader = new BucketSync(httpclient.Object);
            var createdBucket = await reader.CreateEmptyAsync();

            //Act
            createdBucket.Add(new Story());

            //Assert
            httpclient.Verify(mock => mock.PutJsonAsync(BUCKETS, It.IsAny<SyncableBucket>()), Times.Once);
            Assert.AreEqual(createdBucket.Id, updatedId);
        }
    }
}