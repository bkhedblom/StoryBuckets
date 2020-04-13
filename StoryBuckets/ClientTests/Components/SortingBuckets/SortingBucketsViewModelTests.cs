using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StoryBuckets.Client.Models;
using StoryBuckets.Shared;

namespace StoryBuckets.Client.Components.SortingBuckets.Tests
{
    [TestClass()]
    public class SortingBucketsViewModelTests
    {
        [TestMethod()]
        public void TextForNextStoryToSort_IsNotNull()
        {
            //Arrange
            var storylist = new Mock<IStorylist>();
            storylist
                .SetupGet(fake => fake.NextUnbucketedStory)
                .Returns(new Mock<IStory>().Object);

            var vm = new SortingBucketsViewModel(storylist.Object);

            //Act
            //Assert
            Assert.IsNotNull(vm.TextForNextStoryToSort);
        }

        [TestMethod()]
        public void TextForNextStoryToSort_handles_NextUnbucketedStory_being_null()
        {
            //Arrange
            var storylist = new Mock<IStorylist>();
            storylist
                .SetupGet(fake => fake.NextUnbucketedStory)
                .Returns((IStory)null);

            var vm = new SortingBucketsViewModel(storylist.Object);

            //Act
            //Assert
            Assert.IsNotNull(vm.TextForNextStoryToSort);
        }

        [TestMethod()]
        public void TextForNextStoryToSort_is_the_string_representation_of_NextUnbucketedStory()
        {
            //Arrange
            var text = "#1337 Foobar";
            
            var nextUnbucketedStory = new Mock<IStory>();
            nextUnbucketedStory
                .Setup(fake => fake.ToString())
                .Returns(text);
            
            var storylist = new Mock<IStorylist>();
            storylist
                .SetupGet(mock => mock.NextUnbucketedStory)
                .Returns(nextUnbucketedStory.Object);
            
            var vm = new SortingBucketsViewModel(storylist.Object);

            //Act
            //Assert
            Assert.AreEqual(text, vm.TextForNextStoryToSort);
        }

        [TestMethod()]
        public void When_DataIsReady_and_NumberOfUnbucketedStories_is_more_than_0_show_story()
        {
            //Arrange
            var storylist = new Mock<IStorylist>();
            storylist
                .SetupGet(fake => fake.DataIsready)
                .Returns(true);
            storylist
                .SetupGet(fake => fake.NumberOfUnbucketedStories)
                .Returns(3);


            var vm = new SortingBucketsViewModel(storylist.Object);

            //Act
            //Assert
            Assert.IsFalse(vm.HideStory);
        }

        [TestMethod()]
        public void When_DataIsReady_and_NumberOfUnbucketedStories_is_more_than_0_hide_alldone_message()
        {
            //Arrange
            var storylist = new Mock<IStorylist>();
            storylist
                .SetupGet(fake => fake.DataIsready)
                .Returns(true);
            storylist
                .SetupGet(fake => fake.NumberOfUnbucketedStories)
                .Returns(3);

            var vm = new SortingBucketsViewModel(storylist.Object);

            //Act
            //Assert
            Assert.IsTrue(vm.HideAllDone);
        }

        [TestMethod()]
        public void When_DataIsReady_hide_loader()
        {
            //Arrange
            var storylist = new Mock<IStorylist>();
            storylist
                .SetupGet(fake => fake.DataIsready)
                .Returns(true);

            var vm = new SortingBucketsViewModel(storylist.Object);

            //Act
            //Assert
            Assert.IsTrue(vm.HideLoader);
        }

        [TestMethod()]
        public void When_not_DataIsReady_show_loader()
        {
            //Arrange
            var storylist = new Mock<IStorylist>();
            storylist
                .SetupGet(fake => fake.DataIsready)
                .Returns(false);

            var vm = new SortingBucketsViewModel(storylist.Object);

            //Act
            //Assert
            Assert.IsFalse(vm.HideLoader);
        }

        [TestMethod()]
        public void When_not_DataIsReady_hide_story()
        {
            //Arrange
            var storylist = new Mock<IStorylist>();
            storylist
                .SetupGet(fake => fake.DataIsready)
                .Returns(false);

            var vm = new SortingBucketsViewModel(storylist.Object);

            //Act
            //Assert
            Assert.IsTrue(vm.HideStory);
        }

        [TestMethod()]
        public void When_not_DataIsReady_hide_AllDone_message()
        {
            //Arrange
            var storylist = new Mock<IStorylist>();
            storylist
                .SetupGet(fake => fake.DataIsready)
                .Returns(false);

            var vm = new SortingBucketsViewModel(storylist.Object);

            //Act
            //Assert
            Assert.IsTrue(vm.HideAllDone);
        }
    }
}