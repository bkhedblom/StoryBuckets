using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace StoryBuckets.Shared.Tests
{
    [TestClass()]
    public class BucketTests
    {
        [TestMethod()]
        public void Add_AddsStory()
        {
            //Arrange
            var persister = new Mock<IDataPersister<IBucket>>();
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
            var persister = new Mock<IDataPersister<IBucket>>();
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
            var persister = new Mock<IDataPersister<IBucket>>();
            var bucket = new Bucket(persister.Object);
            var story = new Mock<IStory>();

            //Act
            bucket.Add(story.Object);


            //Assert
            persister.Verify(mock => mock.Update(bucket));
        }
    }
}