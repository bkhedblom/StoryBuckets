﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StoryBuckets.Client.ServerCommunication;
using StoryBuckets.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StoryBuckets.Client.Models.Tests
{
    [TestClass()]
    public class StorylistTests
    {
        [TestMethod()]
        public void NextUnbucketedStory_return_null_if_called_before_initialization()
        {
            //Arrange
            var reader = new Mock<IDataReader<SyncableStory>>();
            var storylist = new Storylist(reader.Object);

            //Act
            //Assert
            Assert.IsNull(storylist.NextUnbucketedStory);
        }

        [TestMethod()]
        public void NumberOfUnbucketedStories_return_null_if_called_before_initialization()
        {
            //Arrange
            var reader = new Mock<IDataReader<SyncableStory>>();
            var storylist = new Storylist(reader.Object);

            //Act
            //Assert
            Assert.IsNull(storylist.NumberOfUnbucketedStories);
        }

        [TestMethod()]
        public void DataIsready_should_be_false_before_data_is_loaded()
        {
            //Arrange
            var reader = new Mock<IDataReader<SyncableStory>>();
            var storylist = new Storylist(reader.Object);

            //Act
            //Assert
            Assert.IsFalse(storylist.DataIsready);
        }

        [TestMethod()]
        public void Data_should_be_Read_when_InitializeAsync_is_called()
        {
            //Arrange
            var reader = new Mock<IDataReader<SyncableStory>>();
            var storylist = new Storylist(reader.Object);

            //Act
            storylist.InitializeAsync().Wait();

            //Assert
            reader.Verify(mock => mock.ReadAsync(), Times.Once);
        }

        [TestMethod()]
        public void DataIsready_should_be_true_after_fetching_data()
        {
            //Arrange
            var reader = new Mock<IDataReader<SyncableStory>>();
            reader
                .Setup(fake => fake.ReadAsync())
                .ReturnsAsync(new List<SyncableStory>());

            var storylist = new Storylist(reader.Object);

            //Act
            storylist.InitializeAsync().Wait();

            //Assert
            Assert.IsTrue(storylist.DataIsready);
        }

        [TestMethod()]
        public void NumberOfUnbucketedStories_returns_number_of_stories_not_in_a_bucket()
        {
            //Arrange
            var reader = new Mock<IDataReader<SyncableStory>>();
            var storyInBucket = new SyncableStory
            {
                IsInBucket = true
            };

            var unbucketedStory = new SyncableStory
            {
                IsInBucket = false
            };

            reader
                .Setup(fake => fake.ReadAsync())
                .ReturnsAsync(new[] {
                    storyInBucket,
                    unbucketedStory
                });

            var storylist = new Storylist(reader.Object);

            //Act
            storylist.InitializeAsync().Wait();

            //Assert
            Assert.AreEqual(1u, storylist.NumberOfUnbucketedStories);
        }

        [TestMethod()]
        public void NextUnbucketedStory_returns_an_unbucketedStory_that_is_not_irrelevant()
        {
            //Arrange
            var reader = new Mock<IDataReader<SyncableStory>>();
            var storyInBucket = new SyncableStory
            {
                IsInBucket = true
            };

            var unbucketedStory = new SyncableStory
            {
                IsInBucket = false
            };

            reader
                .Setup(fake => fake.ReadAsync())
                .ReturnsAsync(new[] {
                    storyInBucket,
                    unbucketedStory
                });

            var storylist = new Storylist(reader.Object);

            //Act
            storylist.InitializeAsync().Wait();

            //Assert
            Assert.AreEqual(unbucketedStory, storylist.NextUnbucketedStory);
        }

        [TestMethod()]
        public void NextUnbucketedStory_is_null_if_all_stories_are_in_buckets()
        {
            //Arrange
            var reader = new Mock<IDataReader<SyncableStory>>();
            var storyInBucket = new SyncableStory
            {
                IsInBucket = true
            };

            reader
                .Setup(fake => fake.ReadAsync())
                .ReturnsAsync(new[] {
                    storyInBucket
                });

            var storylist = new Storylist(reader.Object);

            //Act
            storylist.InitializeAsync().Wait();

            //Assert
            Assert.IsNull(storylist.NextUnbucketedStory);
        }

        [TestMethod()]
        public async Task NextUnbucketedStory_does_not_return_an_irrelevant_story()
        {
            //Arrange
            var reader = new Mock<IDataReader<SyncableStory>>();
            var storyInBucket = new SyncableStory
            {
                Id = 273,
                Title = "In Bucket",
                IsInBucket = true
            };

            var irrelevantStory = new SyncableStory
            {
                Id = 314,
                Title = "Irrelevant",
                IsInBucket = false,
                IsIrrelevant = true
            };

            var unbucketedStory = new SyncableStory
            {
                Id = 42,
                Title = "Unbucketed",
                IsInBucket = false
            };

            reader
                .Setup(fake => fake.ReadAsync())
                .ReturnsAsync(new[] {
                    storyInBucket,
                    irrelevantStory,
                    unbucketedStory
                });

            var storylist = new Storylist(reader.Object);

            //Act
            await storylist.InitializeAsync();

            //Assert
            Assert.AreEqual(unbucketedStory, storylist.NextUnbucketedStory);
        }
    }
}