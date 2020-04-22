using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StoryBuckets.Client.Models;
using StoryBuckets.Client.ServerCommunication;
using StoryBuckets.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoryBuckets.Client.Models.Tests
{
    [TestClass()]
    public class SyncableStoryTests
    {
        [TestMethod()]
        public void Constructor_with_story_maps_properties_from_that_story()
        {
            //Arrange
            var story = new Story
            {
                Id = 42,
                IsInBucket = true,
                Title = "foobar"
            };


            //Act
            var model = new SyncableStory(story);

            //Assert
            Assert.AreEqual(story.Id, model.Id);
            Assert.AreEqual(story.IsInBucket, model.IsInBucket);
            Assert.AreEqual(story.Title, model.Title);
        }

        [TestMethod()]
        public void Updated_event_triggered_when_IsInBucket_is_changed()
        {
            //Arrange
            var model = new SyncableStory();
            var updatedTriggered = false;
            model.Updated += (o, e) => updatedTriggered = true;

            //Act
            model.IsInBucket = !model.IsInBucket;

            //Assert
            Assert.IsTrue(updatedTriggered);
        }

        [TestMethod()]
        public void Updated_event_not_triggered_when_IsInBucket_is_set_without_changing_value()
        {
            //Arrange
            var model = new SyncableStory();
            var updatedTriggered = false;
            model.Updated += (o, e) => updatedTriggered = true;

            //Act
            model.IsInBucket = model.IsInBucket;

            //Assert
            Assert.IsFalse(updatedTriggered);
        }

        [TestMethod()]
        public void IsInBucket_value_is_already_Set_when_Updated_event_is_triggered()
        {
            //Arrange
            var model = new SyncableStory();
            var IsInBucketValue = model.IsInBucket;
            model.Updated += (o, e) => IsInBucketValue = (o as SyncableStory).IsInBucket;

            //Act
            model.IsInBucket = !model.IsInBucket;

            //Assert
            Assert.IsTrue(IsInBucketValue);
        }
    }
}