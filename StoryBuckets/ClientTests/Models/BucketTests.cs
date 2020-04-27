using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StoryBuckets.Shared;
using System.Collections.Generic;
using System.Linq;

namespace StoryBuckets.Client.Models.Tests
{
    [TestClass()]
    public class BucketTests
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
        public void Add_SetsBucketOfAddedStory()
        {
            //Arrange
            var bucket = new SyncableBucket();
            var story = new Story();

            //Act
            bucket.Add(story);


            //Assert
            Assert.AreEqual(bucket, story.Bucket);
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
    }
}