using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StoryBuckets.DataStores.Buckets;
using StoryBuckets.DataStores.FileStorage;
using StoryBuckets.DataStores.FileStore;
using StoryBuckets.DataStores.Stories;
using StoryBuckets.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BucketBuckets.DataStores.Buckets.Tests
{
    [TestClass()]
    public class InMemoryFilebackedBucketStoreTests
    {
        [TestMethod()]
        public void Uses_the_StorageFolder_buckets()
        {
            //Arrange
            var folderprovider = new Mock<IStorageFolderProvider>();
            var storyStore = new Mock<IFileBackedStoryDataStore>();

            //Act
            _ = new InMemoryFileBackedBucketDataStore(folderprovider.Object, storyStore.Object);

            //Assert
            folderprovider.Verify(mock => mock.GetStorageFolder<Bucket>("buckets"));
        }

        [TestMethod()]
        public void IsInitialized_starts_out_false()
        {
            //Arrange
            var folderprovider = new Mock<IStorageFolderProvider>();
            var storyStore = new Mock<IFileBackedStoryDataStore>();

            var datastore = new InMemoryFileBackedBucketDataStore(folderprovider.Object, storyStore.Object);

            //Act
            //Assert
            Assert.IsFalse(datastore.IsInitialized);
        }

        [TestMethod()]
        public void IsInitialized_is_true_after_calling_initialize()
        {
            //Arrange
            var storagefolder = new Mock<IStorageFolder<Bucket>>();
            storagefolder
                .Setup(fake => fake.GetStoredItemsAsync())
                .Returns(MakeAsync(new List<Bucket>()));

            var folderprovider = new Mock<IStorageFolderProvider>();
            folderprovider
                .Setup(fake => fake.GetStorageFolder<Bucket>(It.IsAny<string>()))
                .Returns(storagefolder.Object);

            var storyStore = new Mock<IFileBackedStoryDataStore>();

            var datastore = new InMemoryFileBackedBucketDataStore(folderprovider.Object, storyStore.Object);

            //Act
            datastore.InitializeAsync().Wait();

            //Assert
            Assert.IsTrue(datastore.IsInitialized);
        }

        [TestMethod()]
        public void Initializing_gets_the_items_from_StorageFolder()
        {
            //Arrange
            var storagefolder = new Mock<IStorageFolder<Bucket>>();
            storagefolder
                .Setup(fake => fake.GetStoredItemsAsync())
                .Returns(MakeAsync(new List<Bucket>()));

            var folderprovider = new Mock<IStorageFolderProvider>();
            folderprovider
                .Setup(fake => fake.GetStorageFolder<Bucket>(It.IsAny<string>()))
                .Returns(storagefolder.Object);

            var storyStore = new Mock<IFileBackedStoryDataStore>();

            var datastore = new InMemoryFileBackedBucketDataStore(folderprovider.Object, storyStore.Object);

            //Act
            datastore.InitializeAsync().Wait();

            //Assert
            storagefolder.Verify(mock => mock.GetStoredItemsAsync(), Times.Once);
        }

        [TestMethod()]
        public void Initialize_stores_the_retrieved_items_in_the_DataStore()
        {
            //Arrange
            var bucket = new Bucket
            {
                Id = 42
            };

            var storagefolder = new Mock<IStorageFolder<Bucket>>();
            storagefolder
                .Setup(fake => fake.GetStoredItemsAsync())
                .Returns(MakeAsync(new[] { bucket }));

            var folderprovider = new Mock<IStorageFolderProvider>();
            folderprovider
                .Setup(fake => fake.GetStorageFolder<Bucket>(It.IsAny<string>()))
                .Returns(storagefolder.Object);

            var storyStore = new Mock<IFileBackedStoryDataStore>();

            var datastore = new InMemoryFileBackedBucketDataStore(folderprovider.Object, storyStore.Object);

            //Act
            datastore.InitializeAsync().Wait();

            //Assert
            var result = datastore.GetAllAsync().Result;
            Assert.AreEqual(bucket, result.Single());
        }

        [TestMethod()]
        public void Initializing_initializes_StoryStore()
        {
            //Arrange
            var storagefolder = new Mock<IStorageFolder<Bucket>>();
            storagefolder
                .Setup(fake => fake.GetStoredItemsAsync())
                .Returns(MakeAsync(new List<Bucket>()));

            var folderprovider = new Mock<IStorageFolderProvider>();
            folderprovider
                .Setup(fake => fake.GetStorageFolder<Bucket>(It.IsAny<string>()))
                .Returns(storagefolder.Object);

            var storyStore = new Mock<IFileBackedStoryDataStore>();

            var datastore = new InMemoryFileBackedBucketDataStore(folderprovider.Object, storyStore.Object);

            //Act
            datastore.InitializeAsync().Wait();

            //Assert
            storyStore.Verify(mock => mock.InitializeAsync(), Times.Once);
        }

        [TestMethod()]
        public void Initializing_do_not_initialize_already_initialized_StoryStore()
        {
            //Arrange
            var storagefolder = new Mock<IStorageFolder<Bucket>>();
            storagefolder
                .Setup(fake => fake.GetStoredItemsAsync())
                .Returns(MakeAsync(new List<Bucket>()));

            var folderprovider = new Mock<IStorageFolderProvider>();
            folderprovider
                .Setup(fake => fake.GetStorageFolder<Bucket>(It.IsAny<string>()))
                .Returns(storagefolder.Object);

            var storyStore = new Mock<IFileBackedStoryDataStore>();
            storyStore
                .SetupGet(fake => fake.IsInitialized)
                .Returns(true);

            var datastore = new InMemoryFileBackedBucketDataStore(folderprovider.Object, storyStore.Object);

            //Act
            datastore.InitializeAsync().Wait();

            //Assert
            storyStore.Verify(mock => mock.InitializeAsync(), Times.Never);
        }

        [TestMethod()]
        public void Stories_are_fetched_from_StoryStore()
        {
            //Arrange
            var id = 42;
            var bucket = new Bucket
            {
                Id = id
            };

            var storagefolder = new Mock<IStorageFolder<Bucket>>();
            storagefolder
                .Setup(fake => fake.GetStoredItemsAsync())
                .Returns(MakeAsync(new[] { bucket }));

            var folderprovider = new Mock<IStorageFolderProvider>();
            folderprovider
                .Setup(fake => fake.GetStorageFolder<Bucket>(It.IsAny<string>()))
                .Returns(storagefolder.Object);

            var storyStore = new Mock<IFileBackedStoryDataStore>();

            var datastore = new InMemoryFileBackedBucketDataStore(folderprovider.Object, storyStore.Object);


            //Act
            datastore.InitializeAsync().Wait();
            _ = datastore.GetAllAsync().Result;

            //Assert
            storyStore.Verify(mock => mock.GetStoriesInBucket(id), Times.Once);
        }

        [TestMethod()]
        public void Contained_stories_from_StoryStore_are_added_to_Bucket()
        {
            //Arrange
            var id = 42;
            var bucket = new Bucket
            {
                Id = id
            };

            var storagefolder = new Mock<IStorageFolder<Bucket>>();
            storagefolder
                .Setup(fake => fake.GetStoredItemsAsync())
                .Returns(MakeAsync(new[] { bucket }));

            var folderprovider = new Mock<IStorageFolderProvider>();
            folderprovider
                .Setup(fake => fake.GetStorageFolder<Bucket>(It.IsAny<string>()))
                .Returns(storagefolder.Object);

            var stories = new[]
            {
                new Story{ 
                    Id = 4
                },
                new Story
                {
                    Id = 2
                }
            };
            var storyStore = new Mock<IFileBackedStoryDataStore>();
            storyStore
                .Setup(fake => fake.GetStoriesInBucket(It.IsAny<int>()))
                .ReturnsAsync(stories);

            var datastore = new InMemoryFileBackedBucketDataStore(folderprovider.Object, storyStore.Object);

            //Act
            datastore.InitializeAsync().Wait();
            var buckets = datastore.GetAllAsync().Result;

            //Assert
            var storiesOfBucket = buckets.Single().Stories;
            Assert.AreEqual(stories.Length, storiesOfBucket.Count);
            foreach (var fakeStory in stories)
            {
                var bucketContainsStory = storiesOfBucket.Any(returnedStory => returnedStory.Id == fakeStory.Id);
                Assert.IsTrue(bucketContainsStory, $"Story with id {fakeStory.Id} not found in bucket!");
            }
        }

        //[TestMethod()]
        //public void Added_items_can_be_retrieved()
        //{
        //    //Arrange
        //    var storagefolder = new Mock<IStorageFolder<Bucket>>();

        //    var folderprovider = new Mock<IStorageFolderProvider>();
        //    folderprovider
        //        .Setup(fake => fake.GetStorageFolder<Bucket>(It.IsAny<string>()))
        //        .Returns(storagefolder.Object);

        //    var datastore = new InMemoryFileBackedBucketDataStore(folderprovider.Object);

        //    var items = new[]
        //    {
        //        new Bucket()
        //    };

        //    //Act
        //    datastore.AddOrUpdateAsync(items).Wait();
        //    var retrieved = datastore.GetAllAsync().Result;

        //    //Assert
        //    Assert.AreEqual(items.Single(), retrieved.Single());
        //}

        //[TestMethod()]
        //public void The_same_item_is_not_added_twice()
        //{
        //    //Arrange
        //    var storagefolder = new Mock<IStorageFolder<Bucket>>();

        //    var folderprovider = new Mock<IStorageFolderProvider>();
        //    folderprovider
        //        .Setup(fake => fake.GetStorageFolder<Bucket>(It.IsAny<string>()))
        //        .Returns(storagefolder.Object);

        //    var datastore = new InMemoryFileBackedBucketDataStore(folderprovider.Object);

        //    var items = new[]
        //    {
        //        new Bucket()
        //    };

        //    //Act
        //    datastore.AddOrUpdateAsync(items).Wait();
        //    var countAfterFirstAdd = datastore.GetAllAsync().Result.Count();

        //    datastore.AddOrUpdateAsync(items).Wait();
        //    var countAfterSecondAdd = datastore.GetAllAsync().Result.Count();

        //    //Assert
        //    Assert.AreEqual(countAfterFirstAdd, countAfterSecondAdd);
        //}

        //[TestMethod()]
        //public void The_same_Id_exists_only_once()
        //{
        //    //Arrange
        //    var storagefolder = new Mock<IStorageFolder<Bucket>>();

        //    var folderprovider = new Mock<IStorageFolderProvider>();
        //    folderprovider
        //        .Setup(fake => fake.GetStorageFolder<Bucket>(It.IsAny<string>()))
        //        .Returns(storagefolder.Object);

        //    var datastore = new InMemoryFileBackedBucketDataStore(folderprovider.Object);

        //    var id = 42;

        //    var items = new[]
        //    {
        //        new Bucket
        //        {
        //            Id = id
        //        },
        //        new Bucket
        //        {
        //            Id = id
        //        }
        //    };

        //    //Act
        //    datastore.AddOrUpdateAsync(items).Wait();
        //    var retrieved = datastore.GetAllAsync().Result;

        //    //Assert
        //    Assert.AreEqual(1, retrieved.Count(item => item.Id == id));
        //}

        //[TestMethod()]
        //public void Adding_the_same_Id_again_updates_exising_item_to_the_new()
        //{
        //    //Arrange
        //    var storagefolder = new Mock<IStorageFolder<Bucket>>();

        //    var folderprovider = new Mock<IStorageFolderProvider>();
        //    folderprovider
        //        .Setup(fake => fake.GetStorageFolder<Bucket>(It.IsAny<string>()))
        //        .Returns(storagefolder.Object);

        //    var datastore = new InMemoryFileBackedBucketDataStore(folderprovider.Object);

        //    var id = 42;
        //    var item1 = new Bucket
        //    {
        //        Id = id
        //    };

        //    var item2 = new Bucket
        //    {
        //        Id = id
        //    };

        //    //Act
        //    datastore.AddOrUpdateAsync(new[] { item1 }).Wait();
        //    datastore.AddOrUpdateAsync(new[] { item2 }).Wait();
        //    var retrieved = datastore.GetAllAsync().Result;

        //    //Assert
        //    Assert.AreEqual(item2, retrieved.Single(item => item.Id == id));
        //}

        //[TestMethod()]
        //public void Adding_items_creates_new_files_for_them_using_id_as_Filename()
        //{
        //    //Arrange
        //    var bucket1 = new Bucket
        //    {
        //        Id = 42
        //    };

        //    var bucket2 = new Bucket
        //    {
        //        Id = 314
        //    };

        //    var storagefolder = new Mock<IStorageFolder<Bucket>>();

        //    var folderprovider = new Mock<IStorageFolderProvider>();
        //    folderprovider
        //        .Setup(fake => fake.GetStorageFolder<Bucket>(It.IsAny<string>()))
        //        .Returns(storagefolder.Object);

        //    var datastore = new InMemoryFileBackedBucketDataStore(folderprovider.Object);

        //    //Act
        //    datastore.AddOrUpdateAsync(new[] { bucket1, bucket2 }).Wait();

        //    //Assert
        //    storagefolder.Verify(mock => mock.CreateFileForItemAsync(bucket1, bucket1.Id.ToString()));
        //    storagefolder.Verify(mock => mock.CreateFileForItemAsync(bucket2, bucket2.Id.ToString()));
        //}

        //[TestMethod()]
        //public void Adding_item_with_the_same_Id_replaces_current_file()
        //{
        //    //Arrange
        //    var id = 42;
        //    var bucket1 = new Bucket
        //    {
        //        Id = id
        //    };

        //    var bucket2 = new Bucket
        //    {
        //        Id = id
        //    };

        //    var storagefolder = new Mock<IStorageFolder<Bucket>>();

        //    var folderprovider = new Mock<IStorageFolderProvider>();
        //    folderprovider
        //        .Setup(fake => fake.GetStorageFolder<Bucket>(It.IsAny<string>()))
        //        .Returns(storagefolder.Object);

        //    var datastore = new InMemoryFileBackedBucketDataStore(folderprovider.Object);

        //    //Act
        //    datastore.AddOrUpdateAsync(new[] { bucket1 }).Wait();
        //    datastore.AddOrUpdateAsync(new[] { bucket2 }).Wait();

        //    //Assert
        //    storagefolder.Verify(mock => mock.ReplaceFileWithItemAsync(id.ToString(), bucket2));
        //}

        //[TestMethod()]
        //public void Update_updates_the_existing_item()
        //{
        //    //Arrange
        //    var storagefolder = new Mock<IStorageFolder<Bucket>>();

        //    var folderprovider = new Mock<IStorageFolderProvider>();
        //    folderprovider
        //        .Setup(fake => fake.GetStorageFolder<Bucket>(It.IsAny<string>()))
        //        .Returns(storagefolder.Object);

        //    var datastore = new InMemoryFileBackedBucketDataStore(folderprovider.Object);

        //    var id = 42;
        //    var bucket1 = new Bucket
        //    {
        //        Id = id
        //    };

        //    var bucket2 = new Bucket
        //    {
        //        Id = id
        //    };
        //    datastore.AddOrUpdateAsync(new[] { bucket1 }).Wait();

        //    //Act
        //    datastore.UpdateAsync(id, bucket2).Wait();
        //    var retrieved = datastore.GetAllAsync().Result;

        //    //Assert
        //    Assert.AreEqual(bucket2, retrieved.Single(item => item.Id == id));
        //}

        //[TestMethod()]
        //public void Trying_to_update_non_existing_item_throws_InvalidOperationException()
        //{
        //    //Arrange
        //    var storagefolder = new Mock<IStorageFolder<Bucket>>();

        //    var folderprovider = new Mock<IStorageFolderProvider>();
        //    folderprovider
        //        .Setup(fake => fake.GetStorageFolder<Bucket>(It.IsAny<string>()))
        //        .Returns(storagefolder.Object);

        //    var datastore = new InMemoryFileBackedBucketDataStore(folderprovider.Object);

        //    var id = 42;
        //    var bucket1 = new Bucket
        //    {
        //        Id = id
        //    };


        //    //Act &&
        //    //Assert
        //    Assert.ThrowsExceptionAsync<InvalidOperationException>(() => datastore.UpdateAsync(id, bucket1)).Wait();
        //}

        //[TestMethod()]
        //public void Trying_to_set_an_item_with_id_that_differs_from_supplied_id_throws_InvalidOperationException()
        //{
        //    //Arrange
        //    var storagefolder = new Mock<IStorageFolder<Bucket>>();

        //    var folderprovider = new Mock<IStorageFolderProvider>();
        //    folderprovider
        //        .Setup(fake => fake.GetStorageFolder<Bucket>(It.IsAny<string>()))
        //        .Returns(storagefolder.Object);

        //    var datastore = new InMemoryFileBackedBucketDataStore(folderprovider.Object);

        //    var id = 42;
        //    var bucket1 = new Bucket
        //    {
        //        Id = id
        //    };
        //    datastore.AddOrUpdateAsync(new[] { bucket1 }).Wait(); //first, the id must exist

        //    var bucket2 = new Bucket
        //    {
        //        Id = 314
        //    };

        //    //Act &&
        //    //Assert
        //    Assert.ThrowsExceptionAsync<InvalidOperationException>(() => datastore.UpdateAsync(id, bucket2)).Wait();
        //}

        //[TestMethod()]
        //public void Updating_item_updates_corresponding_file()
        //{
        //    //Arrange
        //    var id = 42;
        //    var bucket1 = new Bucket
        //    {
        //        Id = id
        //    };

        //    var bucket2 = new Bucket
        //    {
        //        Id = id
        //    };

        //    var storagefolder = new Mock<IStorageFolder<Bucket>>();

        //    var folderprovider = new Mock<IStorageFolderProvider>();
        //    folderprovider
        //        .Setup(fake => fake.GetStorageFolder<Bucket>(It.IsAny<string>()))
        //        .Returns(storagefolder.Object);

        //    var datastore = new InMemoryFileBackedBucketDataStore(folderprovider.Object);

        //    datastore.AddOrUpdateAsync(new[] { bucket1 }).Wait();
        //    //Act
        //    datastore.UpdateAsync(id, bucket2).Wait();

        //    //Assert
        //    storagefolder.Verify(mock => mock.ReplaceFileWithItemAsync(id.ToString(), bucket2));
        //}

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async IAsyncEnumerable<T> MakeAsync<T>(IEnumerable<T> items)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            foreach (var item in items)
            {
                yield return item;
            }
        }
    }
}