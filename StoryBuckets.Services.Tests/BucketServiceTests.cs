using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StoryBuckets.DataStores;
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
    public class BucketServiceTests
    {
        [TestMethod()]
        public void GetAll_returns_some_buckets()
        {
            //Arrange
            var dataStore = new Mock<IDataStore<Bucket>>();
            dataStore
                .Setup(fake => fake.GetAllAsync())
                .ReturnsAsync(new List<Bucket>());

            var service = new BucketService(datastore: dataStore.Object);

            //Act
            var result = service.GetAllAsync().Result;

            //Assert
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void GetAll_gets_buckets_From_datastore()
        {
            //Arrange
            var dataStore = new Mock<IDataStore<Bucket>>();

            var service = new BucketService(datastore: dataStore.Object);

            //Act
            _ = service.GetAllAsync().Result;

            //Assert
            dataStore.Verify(mock => mock.GetAllAsync(), Times.Once);
        }

        [TestMethod()]
        public void GetAll_returns_buckets_from_datastore()
        {
            //Arrange
            var buckets = new[]
            {
                new Bucket()
            };

            var dataStore = new Mock<IDataStore<Bucket>>();
            dataStore
                .Setup(fake => fake.GetAllAsync())
                .ReturnsAsync(buckets);

            var service = new BucketService(datastore: dataStore.Object);

            //Act
            var result = service.GetAllAsync().Result;

            //Assert
            Assert.AreEqual(buckets.Single(), result.Single());
        }

        [TestMethod()]
        public void If_DataStore_is_not_initialized_GetAll_initializes_it()
        {
            //Arrange
            var dataStore = new Mock<IDataStore<Bucket>>();
            dataStore
                .SetupGet(fake => fake.IsInitialized)
                .Returns(false);

            var service = new BucketService(dataStore.Object);

            //Act
            service.GetAllAsync().Wait();

            //Assert
            dataStore.Verify(mock => mock.InitializeAsync(), Times.Once);
        }

        [TestMethod()]
        public async Task If_DataStore_is_not_initialized_Add_initializes_it()
        {
            //Arrange
            var dataStore = new Mock<IDataStore<Bucket>>();
            dataStore
                .SetupGet(fake => fake.IsInitialized)
                .Returns(false);

            var service = new BucketService(dataStore.Object);

            var bucket = new Bucket();

            //Act
            await service.Add(bucket);

            //Assert
            dataStore.Verify(mock => mock.InitializeAsync(), Times.Once);
        }

        [TestMethod()]
        public async Task Add_only_initializes_datastore_if_needed()
        {
            //Arrange
            var dataStore = new Mock<IDataStore<Bucket>>();
            dataStore
                .SetupGet(fake => fake.IsInitialized)
                .Returns(true);

            var service = new BucketService(dataStore.Object);

            var bucket = new Bucket();

            //Act
            await service.Add(bucket);

            //Assert
            dataStore.Verify(mock => mock.InitializeAsync(), Times.Never);
        }

        [TestMethod()]
        public async Task Add_adds_bucket_to_store()
        {
            //Arrange
            Bucket addedBucket = null;
            var dataStore = new Mock<IDataStore<Bucket>>();
            dataStore
                .Setup(mock => mock.AddOrUpdateAsync(It.IsAny<IEnumerable<Bucket>>()))
                .Callback((IEnumerable<Bucket> buckets) => addedBucket = buckets.Single());

            var service = new BucketService(dataStore.Object);

            var bucket = new Bucket();

            //Act
            await service.Add(bucket);

            //Assert
            Assert.AreEqual(bucket, addedBucket);
        }
    }
}