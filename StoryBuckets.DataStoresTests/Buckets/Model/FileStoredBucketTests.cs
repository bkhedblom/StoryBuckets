using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuGet.Frameworks;
using StoryBuckets.DataStores.Buckets.Model;
using StoryBuckets.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace StoryBuckets.DataStores.Buckets.Model.Tests
{
    [TestClass()]
    public class FileStoredBucketTests
    {
        [TestMethod()]
        public void MapFromData_maps_Id()
        {
            //Arrange
            var data = new Bucket
            {
                Id = 42
            };
            var fileStored = new FileStoredBucket();

            //Act
            fileStored.MapFromData(data);

            //Assert
            Assert.AreEqual(data.Id, fileStored.Id);
        }

        [TestMethod()]
        public void ToData_maps_Id()
        {
            //Arrange
            var fileStored = new FileStoredBucket
            {
                Id = 42
            };

            //Act
            var data = fileStored.ToData();

            //Assert
            Assert.AreEqual(fileStored.Id, data.Id);
        }

        [TestMethod()]
        public void MapFromData_maps_StoryIds_from_Stories()
        {
            var stories = new[]
            {
                new Story { Id = 31415 },
                new Story { Id = 2718 }
            };
            //Arrange
            var data = new Bucket(stories);

            var fileStored = new FileStoredBucket();

            //Act
            fileStored.MapFromData(data);

            //Assert
            foreach (var story in stories)
            {
                var fileStoredContainsId = fileStored.StoryIds.Contains(story.Id);
                Assert.IsTrue(fileStoredContainsId, $"Story id {story.Id} was not mapped");
            }
        }

        [TestMethod()]
        public void ToData_with_stories_maps_id()
        {
            //Arrange
            var fileStored = new FileStoredBucket
            {
                Id = 42
            };

            //Act
            var data = fileStored.ToData(new[] { new Story() });

            //Assert
            Assert.AreEqual(fileStored.Id, data.Id);
        }

        [TestMethod()]
        public void ToData_with_stories_maps_those_stories()
        {
            //Arrange
            var fileStored = new FileStoredBucket();

            var stories = new[] { new Story(), new Story() };

            //Act
            var data = fileStored.ToData(stories);

            //Assert
            Assert.AreEqual(2, data.Stories.Count);
            Assert.AreEqual(stories.First(), data.Stories.First());
            Assert.AreEqual(stories.Last(), data.Stories.Last());
        }
    }
}