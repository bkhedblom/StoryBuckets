using Microsoft.AspNetCore.Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StoryBuckets.Client.Models;
using System.Threading.Tasks;

namespace StoryBuckets.Client.Components.BucketWrapper.Tests
{
    [TestClass()]
    public class BucketWrapperTests
    {
        [TestMethod()]
        public async Task Clicking_choose_triggers_chosen_eventAsync()
        {
            //Arrange
            var component = new BucketWrapper();
            var bucket = new Mock<ISyncableBucket>().Object;
            var eventhandler = new Mock<IHandleEvent>();

#pragma warning disable BL0005 // Component parameter should not be set outside of its component.
            component.Bucket = bucket;
            component.OnChosen = new EventCallback<ISyncableBucket>(eventhandler.Object, null);
#pragma warning restore BL0005 // Component parameter should not be set outside of its component.

            //Act
            await component.OnClickChoose();

            //Assert
            eventhandler.Verify(mock => mock.HandleEventAsync(It.IsAny<EventCallbackWorkItem>(), It.IsAny<object>()));
        }

        [TestMethod()]
        public async Task Clicking_choose_sends_contained_bucket_to_event_handler()
        {
            //Arrange
            var component = new BucketWrapper();
            var bucket = new Mock<ISyncableBucket>().Object;
            ISyncableBucket bucketSentToEvent = null;
            var eventhandler = new Mock<IHandleEvent>();
            eventhandler
                .Setup(mock => mock.HandleEventAsync(It.IsAny<EventCallbackWorkItem>(), It.IsAny<ISyncableBucket>()))
                .Callback((EventCallbackWorkItem _, object b) => bucketSentToEvent = b as ISyncableBucket);

#pragma warning disable BL0005 // Component parameter should not be set outside of its component.
            component.Bucket = bucket;
            component.OnChosen = new EventCallback<ISyncableBucket>(eventhandler.Object, null);
#pragma warning restore BL0005 // Component parameter should not be set outside of its component.

            //Act
            await component.OnClickChoose();

            //Assert
            Assert.AreEqual(bucket, bucketSentToEvent);
        }
    }
}