﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StoryBuckets.Client.Models;
using StoryBuckets.Client.ServerCommunication;
using StoryBuckets.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryBuckets.Client.Models.Tests
{
    [TestClass()]
    public class LinkedBucketsModelTests_CreateEmptyBiggerThan
    {
        [TestMethod()]
        public async Task Calls_DataCreator_to_create_a_new_bucket()
        {
            //Arrange
            var dataCreator = new Mock<IDataCreator<IBucketModel>>();
            dataCreator
                .Setup(fake => fake.CreateEmptyAsync())
                .ReturnsAsync(new Mock<IBucketModel>().Object);

            var smallerBucket = new Mock<IBucketModel>().Object;
            
            var testing = new LinkedBucketModels(dataCreator.Object, new[] { smallerBucket });
            
            //Act
            await testing.CreateEmptyBiggerThan(smallerBucket);

            //Assert
            dataCreator.Verify(mock => mock.CreateEmptyAsync(), Times.Once);
        }

        [TestMethod()]
        public async Task When_smaller_bucket_is_null_the_new_bucket_becomes_the_first()
        {
            //Arrange
            var newBucket = new Mock<IBucketModel>().Object;

            var dataCreator = new Mock<IDataCreator<IBucketModel>>();
            dataCreator
                .Setup(fake => fake.CreateEmptyAsync())
                .ReturnsAsync(newBucket);

            var testing = new LinkedBucketModels(dataCreator.Object, new List<IBucketModel>());

            //Act
            await testing.CreateEmptyBiggerThan(null);

            //Assert
            Assert.AreEqual(newBucket, testing.First());
        }

        [TestMethod()]
        public async Task When_adding_a_new_first_the_current_first_is_set_as_NextBiggerBucket()
        {
            //Arrange
            var newBucket = new Mock<IBucketModel>();

            var dataCreator = new Mock<IDataCreator<IBucketModel>>();
            dataCreator
                .Setup(fake => fake.CreateEmptyAsync())
                .ReturnsAsync(newBucket.Object);

            var testing = new LinkedBucketModels(dataCreator.Object, new[] { new Mock<IBucketModel>().Object });
            
            var currentFirstBucket = testing.First();
            
            //Act
            await testing.CreateEmptyBiggerThan(null);

            //Assert
            newBucket.VerifySet(mock => mock.NextBiggerBucket = currentFirstBucket);
        }

        [TestMethod()]
        public async Task When_a_smaller_bucket_is_given_that_one_gets_the_new_as_NextBiggerBucket()
        {
            //Arrange
            var newBucket = new Mock<IBucketModel>().Object;

            var dataCreator = new Mock<IDataCreator<IBucketModel>>();
            dataCreator
                .Setup(fake => fake.CreateEmptyAsync())
                .ReturnsAsync(newBucket);

            var smallerBucket = new Mock<IBucketModel>();
            var testing = new LinkedBucketModels(dataCreator.Object, new[] { smallerBucket.Object });

            //Act
            await testing.CreateEmptyBiggerThan(smallerBucket.Object);

            //Assert
            smallerBucket.VerifySet(mock => mock.NextBiggerBucket = newBucket);
        }

        [TestMethod()]
        public async Task Throws_InvalidOperationException_if_supplied_bucket_is_not_in_the_collection()
        {
            //Arrange
            var newBucket = new Mock<IBucketModel>();

            var dataCreator = new Mock<IDataCreator<IBucketModel>>();
            dataCreator
                .Setup(fake => fake.CreateEmptyAsync())
                .ReturnsAsync(newBucket.Object);

            var testing = new LinkedBucketModels(dataCreator.Object, new[] { new Mock<IBucketModel>().Object });

            //Act & Assert
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () => await testing.CreateEmptyBiggerThan(new Mock<IBucketModel>().Object));

        }

        [TestMethod()]
        public async Task Newly_added_buckets_can_be_used_as_smallerBucket()
        {
            //Arrange
            var dataCreator = new Mock<IDataCreator<IBucketModel>>();
            dataCreator
                .Setup(fake => fake.CreateEmptyAsync())
                .ReturnsAsync(new Mock<IBucketModel>().Object);

            var testing = new LinkedBucketModels(dataCreator.Object, new[] { new Mock<IBucketModel>().Object });

            await testing.CreateEmptyBiggerThan(null);
            var newlyAdded = testing.First();

            //Act
            await testing.CreateEmptyBiggerThan(newlyAdded);

            //Assert: Success if no exception
        }

        [TestMethod()]
        public async Task The_new_bucket_gets_the_smaller_buckets_existing_NextBiggerBucket()
        {
            //Arrange
            var newBucket = new Mock<IBucketModel>();

            var dataCreator = new Mock<IDataCreator<IBucketModel>>();
            dataCreator
                .Setup(fake => fake.CreateEmptyAsync())
                .ReturnsAsync(newBucket.Object);

            var smallerBucket = new Mock<IBucketModel>();
            var biggerBucket = new Mock<IBucketModel>();
            
            smallerBucket
                .SetupGet(fake => fake.NextBiggerBucket)
                .Returns(biggerBucket.Object);

            var testing = new LinkedBucketModels(dataCreator.Object, new[] { smallerBucket.Object, biggerBucket.Object });

            //Act
            await testing.CreateEmptyBiggerThan(smallerBucket.Object);

            //Assert
            newBucket.VerifySet(mock => mock.NextBiggerBucket = biggerBucket.Object);
        }
    }
}