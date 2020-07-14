using Microsoft.VisualStudio.TestTools.UnitTesting;
using StoryBuckets.DataStores.Stories.Model;
using StoryBuckets.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoryBuckets.DataStores.Stories.Model.Tests
{
    [TestClass()]
    public class FileStoredStoryTests
    {
        [TestMethod()]
        public void MapFromData_maps_Id()
        {
            //Arrange
            var data = new Story
            {
                Id = 42
            };
            var fileStored = new FileStoredStory();

            //Act
            fileStored.MapFromData(data);

            //Assert
            Assert.AreEqual(data.Id, fileStored.Id);
        }

        [TestMethod()]
        public void ToData_maps_Id()
        {
            //Arrange
            var fileStored = new FileStoredStory
            {
                Id = 42
            };

            //Act
            var data = fileStored.ToData();

            //Assert
            Assert.AreEqual(fileStored.Id, data.Id);
        }

        [TestMethod()]
        public void MapFromData_maps_Title()
        {
            //Arrange
            var data = new Story
            {
                Title = "Foobar"
            };
            var fileStored = new FileStoredStory();

            //Act
            fileStored.MapFromData(data);

            //Assert
            Assert.AreEqual(data.Title, fileStored.Title);
        }

        [TestMethod()]
        public void ToData_maps_Title()
        {
            //Arrange
            var fileStored = new FileStoredStory
            {
                Title = "foobar"
            };

            //Act
            var data = fileStored.ToData();

            //Assert
            Assert.AreEqual(fileStored.Title, data.Title);
        }

        [TestMethod()]
        public void MapFromData_maps_IsInBucket()
        {
            //Arrange
            var data = new Story
            {
                IsInBucket = true
            };
            var fileStored = new FileStoredStory();

            //Act
            fileStored.MapFromData(data);

            //Assert
            Assert.AreEqual(data.IsInBucket, fileStored.IsInBucket);
        }

        [TestMethod()]
        public void ToData_maps_IsInBucket()
        {
            //Arrange
            var fileStored = new FileStoredStory
            {
                IsInBucket = true
            };

            //Act
            var data = fileStored.ToData();

            //Assert
            Assert.AreEqual(fileStored.IsInBucket, data.IsInBucket);
        }

        [TestMethod()]
        public void MapFromData_maps_IsIrrelevant()
        {
            //Arrange
            var data = new Story
            {
                IsIrrelevant = true
            };
            var fileStored = new FileStoredStory();

            //Act
            fileStored.MapFromData(data);

            //Assert
            Assert.AreEqual(data.IsIrrelevant, fileStored.IsIrrelevant);
        }

        [TestMethod()]
        public void ToData_maps_IsIrrelevant()
        {
            //Arrange
            var fileStored = new FileStoredStory
            {
                IsIrrelevant = true
            };

            //Act
            var data = fileStored.ToData();

            //Assert
            Assert.AreEqual(fileStored.IsIrrelevant, data.IsIrrelevant);
        }
    }
}