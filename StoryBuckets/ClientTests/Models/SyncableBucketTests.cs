using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StoryBuckets.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StoryBuckets.Client.Models.Tests
{
    [TestClass()]
    public class SyncableBucketTests
    {
        [TestMethod()]
        public void Add_AddsStory()
        {
            //Arrange
            var bucket = new SyncableBucket();
            var story = new Mock<Story>();

            //Act
            var countBefore = bucket.Stories.Count;
            bucket.Add(story.Object);


            //Assert
            Assert.AreEqual(countBefore + 1, bucket.Stories.Count);
        }

        [TestMethod()]
        public void Add_Sets_added_story_IsInBucket()
        {
            //Arrange
            var bucket = new SyncableBucket();
            var story = new Story();
            Assert.IsFalse(story.IsInBucket, "Test assumptions have changed!");

            //Act
            bucket.Add(story);

            //Assert
            Assert.IsTrue(story.IsInBucket);
        }

        [TestMethod()]
        public void Adding_Story_triggers_Updated_event_after_Adding_story()
        {
            //Arrange
            var bucket = new SyncableBucket();
            var story = new Story();

            var eventWasTriggered = false;
            Story singleStoryInBucketWhenEventTriggered = null;
            bucket.Updated += (o, e) => {
                eventWasTriggered = true;
                singleStoryInBucketWhenEventTriggered = (o as SyncableBucket).Stories.Single();
            };

            //Act
            bucket.Add(story);


            //Assert
            Assert.IsTrue(eventWasTriggered);
            Assert.AreEqual(story, singleStoryInBucketWhenEventTriggered);
        }

        [TestMethod()]
        public void Setting_NextBiggerBucket_triggers_update()
        {
            //Arrange
            var bucket = new SyncableBucket();

            var eventWasTriggered = false;

            bucket.Updated += (o, e) => {
                eventWasTriggered = true;
            };

            //Act
            bucket.NextBiggerBucket = new SyncableBucket();


            //Assert
            Assert.IsTrue(eventWasTriggered, "Event was not triggered");
        }

        [TestMethod()]
        public void Triggering_update_happens_after_Setting_the_NextBiggerBucket()
        {
            //Arrange
            var bucket = new SyncableBucket();

            Bucket biggerBucketWhenEventTriggered = null;

            bucket.Updated += (o, e) => {
                biggerBucketWhenEventTriggered = (o as SyncableBucket).NextBiggerBucket;
            };

            var biggerBucket = new SyncableBucket();

            //Act
            bucket.NextBiggerBucket = biggerBucket;


            //Assert
            Assert.AreEqual(biggerBucket, biggerBucketWhenEventTriggered);
        }

        [TestMethod()]
        public void Mapping_from_Bucket_maps_Id()
        {
            //Arrange
            var bucket = new Bucket
            {
                Id = 278
            };

            //Act
            var mapped = new SyncableBucket(bucket);


            //Assert
            Assert.AreEqual(bucket.Id, mapped.Id);
        }

        [TestMethod()]
        public void Mapping_from_Bucket_maps_Stories()
        {
            //Arrange
            var story = new Story { Id = 278, Title = "Foobar", IsInBucket = true };
            var bucket = new Bucket(new[] { story })
            {
                Id = 278                
            };

            //Act
            var mapped = new SyncableBucket(bucket);


            //Assert
            var mappedStory = mapped.Stories.Single();
            Assert.AreEqual(story.Id, mappedStory.Id);
            Assert.AreEqual(story.Title, mappedStory.Title);
            Assert.AreEqual(story.IsInBucket, mappedStory.IsInBucket);
        }

        [TestMethod()]
        public void Mapping_from_Bucket_maps_NextBiggerBucketId()
        {
            //Arrange
            var bucket = new Bucket
            {
                Id = 278,
                NextBiggerBucketId = 314
            };

            //Act
            var mapped = new SyncableBucket(bucket);


            //Assert
            Assert.AreEqual(bucket.NextBiggerBucketId, mapped.NextBiggerBucketId);
        }

        [TestMethod()]
        public void Setting_NextBiggerBucket_sets_NextBiggerBucketId()
        {
            //Arrange
            var bucket = new SyncableBucket();
            var newBiggerBucketId = 278;
            //Act
            bucket.NextBiggerBucket = new SyncableBucket { Id = newBiggerBucketId };


            //Assert
            Assert.AreEqual(newBiggerBucketId, bucket.NextBiggerBucketId);
        }

        [TestMethod()]
        public void Triggering_update_happens_after_Setting_the_NextBiggerBucketId()
        {
            //Arrange
            var bucket = new SyncableBucket();

            int? biggerBucketIdWhenEventTriggered = null;

            bucket.Updated += (o, e) => {
                biggerBucketIdWhenEventTriggered = (o as SyncableBucket).NextBiggerBucketId;
            };

            var biggerBucket = new SyncableBucket { Id = 278 };

            //Act
            bucket.NextBiggerBucket = biggerBucket;


            //Assert
            Assert.AreEqual(biggerBucket.Id, biggerBucketIdWhenEventTriggered);
        }

        [TestMethod()]
        public void Setting_NextBiggerBucketId_manually_is_NotSupported()
        {
            //Arrange
            var bucket = new SyncableBucket();

            //Act && Assert
            Assert.ThrowsException<NotSupportedException>(() => bucket.NextBiggerBucketId = 314);
        }
    }
}