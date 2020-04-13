using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Segment.Exception;
using StoryBuckets.Client.Models;
using StoryBuckets.Client.ServerCommunication;
using StoryBuckets.Shared;
using System;
using System.Collections.Generic;
using System.Text;

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
    }
}