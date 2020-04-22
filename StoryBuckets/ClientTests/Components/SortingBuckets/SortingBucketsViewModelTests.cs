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
                .Returns(new Story());

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
                .Returns((Story)null);

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
            
            var nextUnbucketedStory = new Mock<Story>();
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
            Assert.IsFalse(vm.StoryHidden);
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
            Assert.IsTrue(vm.AllDoneHidden);
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
            Assert.IsTrue(vm.LoaderHidden);
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
            Assert.IsFalse(vm.LoaderHidden);
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
            Assert.IsTrue(vm.StoryHidden);
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
            Assert.IsTrue(vm.AllDoneHidden);
        }

        [TestMethod()]
        public void Initializes_storylist_OnInitializedAsync()
        {
            //Arrange
            var storylist = new Mock<IStorylist>();
            storylist
                .SetupGet(fake => fake.DataIsready)
                .Returns(false);

            var vm = new SortingBucketsViewModel(storylist.Object);

            //Act
            vm.OnInitializedAsync().Wait();

            //Assert
            storylist.Verify(mock => mock.InitializeAsync(), Times.Once);
        }

        [TestMethod()]
        public void Clicking_Next_sets_current_as_being_in_bucket()
        {
            //Arrange
            var nextStory = new Story();            
            var storylist = new Mock<IStorylist>();
            storylist
                .SetupGet(fake => fake.NextUnbucketedStory)
                .Returns(nextStory);

            Assert.IsFalse(nextStory.IsInBucket, "Test preconditions failed!");
            var vm = new SortingBucketsViewModel(storylist.Object);

            //Act
            vm.OnClickBtnNext();

            //Assert
            Assert.IsTrue(nextStory.IsInBucket);
        }

        [TestMethod()]
        public void When_DataIsReady_and_NumberOfUnbucketedStories_is_more_than_0_Enable_next_button()
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
            Assert.IsFalse(vm.BtnNextDisabled);
        }

        [TestMethod()]
        public void When_NumberOfUnbucketedStories_is_0_Disable_next_button()
        {
            //Arrange
            var storylist = new Mock<IStorylist>();
            storylist
                .SetupGet(fake => fake.DataIsready)
                .Returns(true);
            storylist
                .SetupGet(fake => fake.NumberOfUnbucketedStories)
                .Returns(0);

            var vm = new SortingBucketsViewModel(storylist.Object);

            //Act
            //Assert
            Assert.IsTrue(vm.BtnNextDisabled);
        }

        [TestMethod()]
        public void When_Not_DataIsReady_Disable_next_button()
        {
            //Arrange
            var storylist = new Mock<IStorylist>();
            storylist
                .SetupGet(fake => fake.DataIsready)
                .Returns(false);
            storylist
                .SetupGet(fake => fake.NumberOfUnbucketedStories)
                .Returns(3);

            var vm = new SortingBucketsViewModel(storylist.Object);

            //Act
            //Assert
            Assert.IsTrue(vm.BtnNextDisabled);
        }
    }
}