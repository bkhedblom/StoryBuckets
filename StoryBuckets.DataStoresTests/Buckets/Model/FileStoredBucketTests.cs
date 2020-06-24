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

        [TestMethod()]
        public void MapFromData_maps_Id_from_NextBiggerBucket()
        {
            //Arrange
            var biggerBucketId = 42;
            var data = new Bucket
            {
                NextBiggerBucket = new Bucket { Id = biggerBucketId }
            };
            var fileStored = new FileStoredBucket();

            //Act
            fileStored.MapFromData(data);

            //Assert
            Assert.AreEqual(biggerBucketId, fileStored.NextBiggerBucketId);
        }

        [TestMethod()]
        public void ToData_makes_sure_accessing_NextBiggerBucket_is_an_InvalidOperationException_if_it_has_not_been_set_when_it_should()
        {
            //Arrange
            var fileStored = new FileStoredBucket
            {
                NextBiggerBucketId = 42
            };

            //Act
            var data = fileStored.ToData();

            //Assert
            Assert.ThrowsException<InvalidOperationException>(() => _ = data.NextBiggerBucket);
        }

        [TestMethod()]
        public void ToData_makes_sure_accessing_null_NextBiggerBucket_is_allowed_when_it_should_be()
        {
            //Arrange
            var fileStored = new FileStoredBucket();

            //Act
            var data = fileStored.ToData();

            //Assert
            _ = data.NextBiggerBucket; //succeed if no exception thrown
        }

        [TestMethod()]
        public void ToData_makes_sure_accessing_NextBiggerBucket_is_allowed_when_correctly_set()
        {
            //Arrange
            const int biggerBucketId = 42;
            var fileStored = new FileStoredBucket
            {
                NextBiggerBucketId = biggerBucketId
            };

            //Act
            var data = fileStored.ToData();
            data.NextBiggerBucket = new Bucket { Id = biggerBucketId };

            //Assert
            _ = data.NextBiggerBucket; //succeed if no exception thrown
        }

        [TestMethod()]
        public void ToData_makes_sure_trying_to_set_NextBiggerBucket_to_bucket_with_wring_Id_is_an_InvalidOperationException()
        {
            //Arrange
            var fileStored = new FileStoredBucket
            {
                NextBiggerBucketId = 42
            };

            var wrongBucket = new Bucket { Id = 314 };

            //Act
            var data = fileStored.ToData();

            //Assert
            Assert.ThrowsException<InvalidOperationException>(() => _ = data.NextBiggerBucket = wrongBucket);
        }
    }
}