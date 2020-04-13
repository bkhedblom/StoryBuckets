using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StoryBuckets.Client.Components.SortingBuckets;
using StoryBuckets.Client.Models;
using StoryBuckets.Shared;
using System;
using System.Collections.Generic;
using System.Text;

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
        public void When_NextUnbucketedStory_exists_show_it_and_hide_the_AllDone_message()
        {
            //Arrange
            var storylist = new Mock<IStorylist>();
            storylist
                .SetupGet(fake => fake.NextUnbucketedStory)
                .Returns(new Mock<IStory>().Object);

            var vm = new SortingBucketsViewModel(storylist.Object);

            //Act
            //Assert
            Assert.IsFalse(vm.HideStory);
            Assert.IsTrue(vm.HideAllDone);
        }

        [TestMethod()]
        public void When_NextUnbucketedStory_is_null_hide_the_story_and_show_the_AllDone_message()
        {
            //Arrange
            var storylist = new Mock<IStorylist>();
            storylist
                .SetupGet(fake => fake.NextUnbucketedStory)
                .Returns((IStory)null);

            var vm = new SortingBucketsViewModel(storylist.Object);

            //Act
            //Assert
            Assert.IsTrue(vm.HideStory);
            Assert.IsFalse(vm.HideAllDone);
        }
    }
}