﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StoryBuckets.Client.Models;
using StoryBuckets.Client.ServerCommunication;
using StoryBuckets.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

            var bucketReader = new Mock<IDataReader<IBucketModel>>();

            var vm = new SortingBucketsViewModel(storylist.Object, bucketReader.Object);

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

            var bucketReader = new Mock<IDataReader<IBucketModel>>();

            var vm = new SortingBucketsViewModel(storylist.Object, bucketReader.Object);

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

            var bucketReader = new Mock<IDataReader<IBucketModel>>();

            var vm = new SortingBucketsViewModel(storylist.Object, bucketReader.Object);

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


            var bucketReader = new Mock<IDataReader<IBucketModel>>();

            var vm = new SortingBucketsViewModel(storylist.Object, bucketReader.Object);

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

            var bucketReader = new Mock<IDataReader<IBucketModel>>();

            var vm = new SortingBucketsViewModel(storylist.Object, bucketReader.Object);

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

            var bucketReader = new Mock<IDataReader<IBucketModel>>();
            bucketReader
                .Setup(fake => fake.ReadAsync())
                .ReturnsAsync(new List<IBucketModel>());

            var vm = new SortingBucketsViewModel(storylist.Object, bucketReader.Object);

            //Act
            vm.OnInitializedAsync();

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

            var bucketReader = new Mock<IDataReader<IBucketModel>>();

            var vm = new SortingBucketsViewModel(storylist.Object, bucketReader.Object);

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

            var bucketReader = new Mock<IDataReader<IBucketModel>>();

            var vm = new SortingBucketsViewModel(storylist.Object, bucketReader.Object);

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

            var bucketReader = new Mock<IDataReader<IBucketModel>>();

            var vm = new SortingBucketsViewModel(storylist.Object, bucketReader.Object);

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

            var bucketReader = new Mock<IDataReader<IBucketModel>>();

            var vm = new SortingBucketsViewModel(storylist.Object, bucketReader.Object);

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
            var bucketReader = new Mock<IDataReader<IBucketModel>>();

            var vm = new SortingBucketsViewModel(storylist.Object, bucketReader.Object);

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

            var bucketReader = new Mock<IDataReader<IBucketModel>>();

            var vm = new SortingBucketsViewModel(storylist.Object, bucketReader.Object);

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

            var bucketReader = new Mock<IDataReader<IBucketModel>>();

            var vm = new SortingBucketsViewModel(storylist.Object, bucketReader.Object);

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

            var bucketReader = new Mock<IDataReader<IBucketModel>>();

            var vm = new SortingBucketsViewModel(storylist.Object, bucketReader.Object);

            //Act
            //Assert
            Assert.IsTrue(vm.BtnNextDisabled);
        }

        [TestMethod()]
        public void Read_buckets_OnInitializedAsync()
        {
            //Arrange
            var storylist = new Mock<IStorylist>();

            var bucketReader = new Mock<IDataReader<IBucketModel>>();

            var vm = new SortingBucketsViewModel(storylist.Object, bucketReader.Object);

            //Act
            vm.OnInitializedAsync().Wait();

            //Assert
            bucketReader.Verify(mock => mock.ReadAsync(), Times.Once);
        }

        [TestMethod()]
        public void Read_buckets_populates_Buckets_property()
        {
            //Arrange
            var storylist = new Mock<IStorylist>();

            var bucket1 = new Mock<IBucketModel>().Object;
            var buckets = new[]
            {
                bucket1,
                new Mock<IBucketModel>().Object,
                new Mock<IBucketModel>().Object
            };
            var bucketReader = new Mock<IDataReader<IBucketModel>>();
            bucketReader
                .Setup(fake => fake.ReadAsync())
                .ReturnsAsync(buckets);

            var vm = new SortingBucketsViewModel(storylist.Object, bucketReader.Object);

            //Act
            vm.OnInitializedAsync().Wait();

            //Assert
            Assert.AreEqual(buckets.Count(), vm.Buckets.Count());
            Assert.AreEqual(bucket1, vm.Buckets.First());
        }

        [TestMethod()]
        public void Loader_shown_until_Buckets_has_been_loaded()
        {
            //Arrange
            var storylist = new Mock<IStorylist>();
            storylist
                .SetupGet(fake => fake.DataIsready)
                .Returns(true);

            var bucketTcs = new TaskCompletionSource<IReadOnlyCollection<IBucketModel>>();
            var bucketReader = new Mock<IDataReader<IBucketModel>>();
            bucketReader
                .Setup(fake => fake.ReadAsync())               
                .Returns(bucketTcs.Task);

            var vm = new SortingBucketsViewModel(storylist.Object, bucketReader.Object);

            //Act
            var initialized = vm.OnInitializedAsync();
            Thread.Sleep(0); //make sure the code gets a chance to run

            //Assert
            Assert.IsFalse(vm.LoaderHidden);
            bucketTcs.SetResult(new List<IBucketModel>());
            Thread.Sleep(0); //make sure the code gets a chance to run
            Assert.IsTrue(vm.LoaderHidden);
            initialized.Wait();
        }

        [TestMethod()]
        public void BucketsHidden_until_Buckets_data_has_been_loaded()
        {
            //Arrange
            var storylist = new Mock<IStorylist>();
            storylist
                .SetupGet(fake => fake.DataIsready)
                .Returns(true);

            var bucketTcs = new TaskCompletionSource<IReadOnlyCollection<IBucketModel>>();
            var bucketReader = new Mock<IDataReader<IBucketModel>>();
            bucketReader
                .Setup(fake => fake.ReadAsync())
                .Returns(bucketTcs.Task);

            var vm = new SortingBucketsViewModel(storylist.Object, bucketReader.Object);

            //Act
            var initialized = vm.OnInitializedAsync();
            Thread.Sleep(0); //make sure the code gets a chance to run

            //Assert
            Assert.IsTrue(vm.BucketsHidden);
            bucketTcs.SetResult(new List<IBucketModel>());
            Thread.Sleep(0); //make sure the code gets a chance to run
            Assert.IsFalse(vm.BucketsHidden);
            initialized.Wait();
        }
    }
}