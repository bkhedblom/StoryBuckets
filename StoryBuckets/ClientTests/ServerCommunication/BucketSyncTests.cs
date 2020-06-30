using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        public async Task ReadAsync_fetches_Buckets_from_the_buckets_endpoint_and_returns_SyncableBuckets()
        {
            //Arrange
            var story = new Story { Id = 278, Title = "Foobar", IsInBucket = true };
            var stories = new[]
            {
                story
            };
            var buckets = new[]
            {
                new Bucket{Id = 1, Stories = stories}
            };

            var httpclient = new Mock<IHttpClient>();
            httpclient
                .Setup(mock => mock.GetJsonAsync<Bucket[]>(It.IsAny<string>()))
                .ReturnsAsync(buckets);

            var reader = new BucketSync(httpclient.Object);

            //Act
            var result = await reader.ReadAsync();

            //Assert
            httpclient.Verify(mock => mock.GetJsonAsync<Bucket[]>(BUCKETS), Times.Once);
            var singleBucketInResult = result.Single();
            Assert.IsInstanceOfType(singleBucketInResult, typeof(SyncableBucket));
            Assert.AreEqual(buckets.Single().Id, singleBucketInResult.Id);
        }

        [TestMethod]
        public async Task ReadAsync_sets_the_NextBiggerBucket_on_SyncableBucket_based_on_NextBiggerBucketId_of_corresponding_Bucket()
        {
            //Arrange
            var smallBucketId = 1;
            var biggerBucketId = 278;
            var buckets = new[]
            {
                new Bucket {Id = smallBucketId, NextBiggerBucketId = biggerBucketId},
                new Bucket {Id = biggerBucketId}
            };

            var httpclient = new Mock<IHttpClient>();
            httpclient
                .Setup(mock => mock.GetJsonAsync<Bucket[]>(It.IsAny<string>()))
                .ReturnsAsync(buckets);

            var reader = new BucketSync(httpclient.Object);

            //Act
            var result = await reader.ReadAsync();

            //Assert
            var smallBucket = result.Single(bucket => bucket.Id == smallBucketId);
            Assert.IsNotNull(smallBucket.NextBiggerBucket);
            Assert.AreEqual(biggerBucketId, smallBucket.NextBiggerBucket.Id);
            var biggerBucket = result.Single(bucket => bucket.Id == biggerBucketId);
            Assert.IsNull(biggerBucket.NextBiggerBucket);
        }

        [TestMethod]
        public async Task CreateEmpty_creates_a_new_Bucket_and_returns_a_SyncableBucket()
        {
            //Arrange
            var httpclient = new Mock<IHttpClient>();
            httpclient
                .Setup(fake => fake.PostJsonAsync(BUCKETS, It.IsAny<Bucket>()))
                .ReturnsAsync(new Bucket { Id = 42 });
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
            httpclient
                .Setup(fake => fake.PostJsonAsync(BUCKETS, It.IsAny<Bucket>()))
                .ReturnsAsync(new Bucket { Id = 42 });
            var syncer = new BucketSync(httpclient.Object);

            //Act
            _ = await syncer.CreateEmptyAsync();

            //Assert
            httpclient.Verify(mock => mock.PostJsonAsync(BUCKETS, It.IsAny<Bucket>()), Times.Once);
        }

        [TestMethod]
        public async Task CreateEmpty_sets_the_returned_id_on_the_returned_bucket()
        {
            //Arrange
            var idForNew = 42;
            var httpclient = new Mock<IHttpClient>();
            httpclient
                .Setup(fake => fake.PostJsonAsync(BUCKETS, It.IsAny<Bucket>()))
                .ReturnsAsync(new Bucket { Id = idForNew });
            var syncer = new BucketSync(httpclient.Object);

            //Act
            var created = await syncer.CreateEmptyAsync();

            //Assert
            Assert.AreEqual(idForNew, created.Id);
        }

        [TestMethod()]
        public async Task ReadAsync_buckets_are_PUT_to_the_server_when_updated()
        {
            //Arrange
            var id1 = 2718;
            var id2 = 31415;
            var buckets = new[]
            {
                new Bucket { Id = id1 },
                new Bucket { Id = id2 }
            };

            var updatedIds = new List<int>();

            var httpclient = new Mock<IHttpClient>();
            httpclient
                .Setup(fake => fake.GetJsonAsync<Bucket[]>(It.IsAny<string>()))
                .ReturnsAsync(buckets);
            httpclient
                .Setup(mock => mock.PutJsonAsync(It.IsAny<string>(), It.IsAny<Bucket>()))
                .Callback<string, Bucket>((_, updatedBucket) => updatedIds.Add(updatedBucket.Id));


            var reader = new BucketSync(httpclient.Object);
            var fetchedBuckets = await reader.ReadAsync();
            Assert.AreEqual(buckets.Count(), fetchedBuckets.Count(), "Test aborted because the reading precondition did not work as expected");

            //Act
            foreach (var bucket in fetchedBuckets)
            {
                bucket.Add(new Story());
            }

            //Assert
            httpclient.Verify(mock => mock.PutJsonAsync(BUCKETS, It.IsAny<Bucket>()), Times.Exactly(buckets.Count()));
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
                .Setup(fake => fake.PostJsonAsync(BUCKETS, It.IsAny<Bucket>()))
                .ReturnsAsync(new Bucket { Id = 42 });
            httpclient
                .Setup(mock => mock.PutJsonAsync(It.IsAny<string>(), It.IsAny<Bucket>()))
                .Callback<string, Bucket>((_, updatedBucket) => updatedId = updatedBucket.Id);


            var reader = new BucketSync(httpclient.Object);
            var createdBucket = await reader.CreateEmptyAsync();

            //Act
            createdBucket.Add(new Story());

            //Assert
            httpclient.Verify(mock => mock.PutJsonAsync(BUCKETS, It.IsAny<Bucket>()), Times.Once);
            Assert.AreEqual(createdBucket.Id, updatedId);
        }

        [TestMethod()]
        public async Task ReadLinkedBuckets_Returns_LinkedBucketModels_based_on_buckets_fetched_from_the_buckets_endpoint()
        {
            //Arrange
            var bigBucket = new Bucket { Id = 1337 };
            var mediumBucket = new Bucket { Id = 42, NextBiggerBucketId = bigBucket.Id };
            var smallBucket = new Bucket { Id = 8, NextBiggerBucketId = mediumBucket.Id };
            var buckets = new[]
            {
                mediumBucket,
                bigBucket,
                smallBucket
            };

            var httpclient = new Mock<IHttpClient>();
            httpclient
                .Setup(mock => mock.GetJsonAsync<Bucket[]>(It.IsAny<string>()))
                .ReturnsAsync(buckets);

            var reader = new BucketSync(httpclient.Object);

            //Act
            var result = await reader.ReadLinkedBucketsAsync();

            //Assert
            httpclient.Verify(mock => mock.GetJsonAsync<Bucket[]>(BUCKETS), Times.Once);
            foreach (var bucket in result)
            {
                Assert.IsTrue(buckets.Contains(bucket));
            }
            foreach (var bucket in buckets)
            {
                Assert.IsTrue(result.Contains(bucket));
            }
        }

        [TestMethod()]
        public async Task Read_LinkedBuckets_are_PUT_to_the_server_when_updated()
        {
            //Arrange
            var bigBucket = new Bucket { Id = 1337 };
            var mediumBucket = new Bucket { Id = 42, NextBiggerBucketId = bigBucket.Id };
            var smallBucket = new Bucket { Id = 8, NextBiggerBucketId = mediumBucket.Id };
            var buckets = new[]
            {
                mediumBucket,
                bigBucket,
                smallBucket
            };

            var updatedIds = new List<int>();

            var httpclient = new Mock<IHttpClient>();
            httpclient
                .Setup(fake => fake.GetJsonAsync<Bucket[]>(It.IsAny<string>()))
                .ReturnsAsync(buckets);
            httpclient
                .Setup(mock => mock.PutJsonAsync(It.IsAny<string>(), It.IsAny<Bucket>()))
                .Callback<string, Bucket>((_, updatedBucket) => updatedIds.Add(updatedBucket.Id));


            var reader = new BucketSync(httpclient.Object);
            var fetchedBuckets = await reader.ReadLinkedBucketsAsync();
            Assert.AreEqual(buckets.Count(), fetchedBuckets.Count(), "Test aborted because the reading precondition did not work as expected");

            //Act
            foreach (var bucket in fetchedBuckets)
            {
                bucket.Add(new Story());
            }

            //Assert
            httpclient.Verify(mock => mock.PutJsonAsync(BUCKETS, It.IsAny<Bucket>()), Times.Exactly(buckets.Count()));
            foreach (var bucket in buckets)
            {
                Assert.IsTrue(updatedIds.Contains(bucket.Id), $"Bucket Id {bucket.Id} was not updated!");
            }
        }
    }
}