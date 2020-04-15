using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StoryBuckets.Client.ServerCommunication;
using StoryBuckets.Shared;

namespace StoryBuckets.Client.Models.Tests
{
    [TestClass()]
    public class BucketTests
    {
        [TestMethod()]
        public void Add_AddsStory()
        {
            //Arrange
            var persister = new Mock<IDataSync<IBucketModel>>();
            var bucket = new Bucket(persister.Object);
            var story = new Mock<IStory>();

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
            var persister = new Mock<IDataSync<IBucketModel>>();
            var bucket = new Bucket(persister.Object);
            var story = new Mock<IStory>();

            //Act
            bucket.Add(story.Object);


            //Assert
            story.VerifySet(mock => mock.Bucket = bucket);
        }

        [TestMethod()]
        public void Add_UpdatePersistedBucket()
        {
            //Arrange
            var persister = new Mock<IDataSync<IBucketModel>>();
            var bucket = new Bucket(persister.Object);
            var story = new Mock<IStory>();

            //Act
            bucket.Add(story.Object);


            //Assert
            persister.Verify(mock => mock.Update(bucket));
        }
    }
}