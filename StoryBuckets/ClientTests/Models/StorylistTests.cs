using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StoryBuckets.Client.ServerCommunication;
using StoryBuckets.Shared;
using System.Collections.Generic;

namespace StoryBuckets.Client.Models.Tests
{
    [TestClass()]
    public class StorylistTests
    {
        [TestMethod()]
        public void NextUnbucketedStory_return_null_if_called_before_initialization()
        {
            //Arrange
            var reader = new Mock<IDataReader<IStory>>();
            var storylist = new Storylist(reader.Object);

            //Act
            //Assert
            Assert.IsNull(storylist.NextUnbucketedStory);
        }

        [TestMethod()]
        public void NumberOfUnbucketedStories_return_null_if_called_before_initialization()
        {
            //Arrange
            var reader = new Mock<IDataReader<IStory>>();
            var storylist = new Storylist(reader.Object);

            //Act
            //Assert
            Assert.IsNull(storylist.NumberOfUnbucketedStories);
        }

        [TestMethod()]
        public void DataIsready_should_be_false_before_data_is_loaded()
        {
            //Arrange
            var reader = new Mock<IDataReader<IStory>>();
            var storylist = new Storylist(reader.Object);

            //Act
            //Assert
            Assert.IsFalse(storylist.DataIsready);
        }

        [TestMethod()]
        public void Data_should_be_Read_when_InitializeAsync_is_called()
        {
            //Arrange
            var reader = new Mock<IDataReader<IStory>>();
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
            var reader = new Mock<IDataReader<IStory>>();
            reader
                .Setup(fake => fake.ReadAsync())
                .ReturnsAsync(new List<IStory>());

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
            var reader = new Mock<IDataReader<IStory>>();
            var storyInBucket = new Mock<IStory>();
            storyInBucket
                .SetupGet(fake => fake.IsInBucket)
                .Returns(true);
            var unbucketedStory = new Mock<IStory>();
            unbucketedStory
                .SetupGet(fake => fake.IsInBucket)
                .Returns(false);
            reader
                .Setup(fake => fake.ReadAsync())
                .ReturnsAsync(new[] { 
                    storyInBucket.Object,
                    unbucketedStory.Object
                });

            var storylist = new Storylist(reader.Object);

            //Act
            storylist.InitializeAsync().Wait();

            //Assert
            Assert.AreEqual(1u, storylist.NumberOfUnbucketedStories);
        }

        [TestMethod()]
        public void NextUnbucketedStory_returns_an_unbucketedStory()
        {
            //Arrange
            var reader = new Mock<IDataReader<IStory>>();
            var storyInBucket = new Mock<IStory>();
            storyInBucket
                .SetupGet(fake => fake.IsInBucket)
                .Returns(true);
            var unbucketedStory = new Mock<IStory>();
            unbucketedStory
                .SetupGet(fake => fake.IsInBucket)
                .Returns(false);
            reader
                .Setup(fake => fake.ReadAsync())
                .ReturnsAsync(new[] {
                    storyInBucket.Object,
                    unbucketedStory.Object
                });

            var storylist = new Storylist(reader.Object);

            //Act
            storylist.InitializeAsync().Wait();

            //Assert
            Assert.AreEqual(unbucketedStory.Object, storylist.NextUnbucketedStory);
        }

        [TestMethod()]
        public void NextUnbucketedStory_is_null_if_all_stories_are_in_buckets()
        {
            //Arrange
            var reader = new Mock<IDataReader<IStory>>();
            var storyInBucket = new Mock<IStory>();
            storyInBucket
                .SetupGet(fake => fake.IsInBucket)
                .Returns(true);
            reader
                .Setup(fake => fake.ReadAsync())
                .ReturnsAsync(new[] {
                    storyInBucket.Object
                });

            var storylist = new Storylist(reader.Object);

            //Act
            storylist.InitializeAsync().Wait();

            //Assert
            Assert.IsNull(storylist.NextUnbucketedStory);
        }
    }
}