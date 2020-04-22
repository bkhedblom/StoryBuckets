using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StoryBuckets.DataStores.FileStorage;
using StoryBuckets.DataStores.FileStore;
using StoryBuckets.DataStores.Stories;
using StoryBuckets.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StoryBuckets.DataStores.Tests
{
    [TestClass()]
    public class InMemoryFileBackedStoryDataStoreTests
    {
        [TestMethod()]
        public void Added_items_can_be_retrieved()
        {
            //Arrange
            var storagefolder = new Mock<IStorageFolder<Story>>();

            var folderprovider = new Mock<IStorageFolderProvider>();
            folderprovider
                .Setup(fake => fake.GetStorageFolder<Story>(It.IsAny<string>()))
                .Returns(storagefolder.Object);

            var datastore = new InMemoryFileBackedStoryDataStore(folderprovider.Object);

            var items = new[]
            {
                new Story()
            };

            //Act
            datastore.AddOrUpdateAsync(items).Wait();
            var retrieved = datastore.GetAllAsync().Result;

            //Assert
            Assert.AreEqual(items.Single(), retrieved.Single());
        }

        [TestMethod()]
        public void The_same_item_is_not_added_twice()
        {
            //Arrange
            var storagefolder = new Mock<IStorageFolder<Story>>();

            var folderprovider = new Mock<IStorageFolderProvider>();
            folderprovider
                .Setup(fake => fake.GetStorageFolder<Story>(It.IsAny<string>()))
                .Returns(storagefolder.Object);

            var datastore = new InMemoryFileBackedStoryDataStore(folderprovider.Object);

            var items = new[]
            {
                new Story()
            };

            //Act
            datastore.AddOrUpdateAsync(items).Wait();
            var countAfterFirstAdd = datastore.GetAllAsync().Result.Count();

            datastore.AddOrUpdateAsync(items).Wait();
            var countAfterSecondAdd = datastore.GetAllAsync().Result.Count();

            //Assert
            Assert.AreEqual(countAfterFirstAdd, countAfterSecondAdd);
        }

        [TestMethod()]
        public void The_same_Id_exists_only_once()
        {
            //Arrange
            var storagefolder = new Mock<IStorageFolder<Story>>();

            var folderprovider = new Mock<IStorageFolderProvider>();
            folderprovider
                .Setup(fake => fake.GetStorageFolder<Story>(It.IsAny<string>()))
                .Returns(storagefolder.Object);

            var datastore = new InMemoryFileBackedStoryDataStore(folderprovider.Object);

            var id = 42;

            var items = new[]
            {
                new Story
                {
                    Id = id
                },
                new Story
                {
                    Id = id
                }
            };

            //Act
            datastore.AddOrUpdateAsync(items).Wait();
            var retrieved = datastore.GetAllAsync().Result;

            //Assert
            Assert.AreEqual(1, retrieved.Count(item => item.Id == id));
        }

        [TestMethod()]
        public void Adding_the_same_Id_again_updates_exising_item_to_the_new()
        {
            //Arrange
            var storagefolder = new Mock<IStorageFolder<Story>>();

            var folderprovider = new Mock<IStorageFolderProvider>();
            folderprovider
                .Setup(fake => fake.GetStorageFolder<Story>(It.IsAny<string>()))
                .Returns(storagefolder.Object);

            var datastore = new InMemoryFileBackedStoryDataStore(folderprovider.Object);

            var id = 42;
            var item1 = new Story
            {
                Id = id
            };

            var item2 = new Story
            {
                Id = id
            };

            //Act
            datastore.AddOrUpdateAsync(new[] { item1 }).Wait();
            datastore.AddOrUpdateAsync(new[] { item2 }).Wait();
            var retrieved = datastore.GetAllAsync().Result;

            //Assert
            Assert.AreEqual(item2, retrieved.Single(item => item.Id == id));
        }

        [TestMethod()]
        public void IsInitialized_starts_out_false()
        {
            //Arrange
            var folderprovider = new Mock<IStorageFolderProvider>();
            var datastore = new InMemoryFileBackedStoryDataStore(folderprovider.Object);

            //Act
            //Assert
            Assert.IsFalse(datastore.IsInitialized);
        }

        [TestMethod()]
        public void IsInitialized_is_true_after_calling_initialize()
        {
            //Arrange
            var storagefolder = new Mock<IStorageFolder<Story>>();
            storagefolder
                .Setup(fake => fake.GetStoredItemsAsync())
                .Returns(MakeAsync(new List<Story>()));

            var folderprovider = new Mock<IStorageFolderProvider>();
            folderprovider
                .Setup(fake => fake.GetStorageFolder<Story>(It.IsAny<string>()))
                .Returns(storagefolder.Object);

            var datastore = new InMemoryFileBackedStoryDataStore(folderprovider.Object);
            //Act
            datastore.InitializeAsync().Wait();

            //Assert
            Assert.IsTrue(datastore.IsInitialized);
        }

        [TestMethod()]
        public void Uses_the_StorageFolder_stories()
        {
            //Arrange
            var folderprovider = new Mock<IStorageFolderProvider>();

            //Act
            _ = new InMemoryFileBackedStoryDataStore(folderprovider.Object);

            //Assert
            folderprovider.Verify(mock => mock.GetStorageFolder<Story>("stories"));
        }

        [TestMethod()]
        public void Initializing_gets_the_items_from_StorageFolder()
        {
            //Arrange
            var storagefolder = new Mock<IStorageFolder<Story>>();
            storagefolder
                .Setup(fake => fake.GetStoredItemsAsync())
                .Returns(MakeAsync(new List<Story>()));

            var folderprovider = new Mock<IStorageFolderProvider>();
            folderprovider
                .Setup(fake => fake.GetStorageFolder<Story>(It.IsAny<string>()))
                .Returns(storagefolder.Object);

            var datastore = new InMemoryFileBackedStoryDataStore(folderprovider.Object);

            //Act
            datastore.InitializeAsync().Wait();

            //Assert
            storagefolder.Verify(mock => mock.GetStoredItemsAsync(), Times.Once);
        }

        [TestMethod()]
        public void Initialize_stores_the_retrieved_items_in_the_DataStore()
        {
            //Arrange
            var story = new Story
            {
                Id = 42
            };

            var storagefolder = new Mock<IStorageFolder<Story>>();
            storagefolder
                .Setup(fake => fake.GetStoredItemsAsync())
                .Returns(MakeAsync(new[] { story }));

            var folderprovider = new Mock<IStorageFolderProvider>();
            folderprovider
                .Setup(fake => fake.GetStorageFolder<Story>(It.IsAny<string>()))
                .Returns(storagefolder.Object);

            var datastore = new InMemoryFileBackedStoryDataStore(folderprovider.Object);

            //Act
            datastore.InitializeAsync().Wait();

            //Assert
            var result = datastore.GetAllAsync().Result;
            Assert.AreEqual(story, result.Single());
        }

        [TestMethod()]
        public void Adding_items_creates_new_files_for_them_using_id_as_Filename()
        {
            //Arrange
            var story1 = new Story
            {
                Id = 42
            };

            var story2 = new Story
            {
                Id = 314
            };

            var storagefolder = new Mock<IStorageFolder<Story>>();
            
            var folderprovider = new Mock<IStorageFolderProvider>();
            folderprovider
                .Setup(fake => fake.GetStorageFolder<Story>(It.IsAny<string>()))
                .Returns(storagefolder.Object);

            var datastore = new InMemoryFileBackedStoryDataStore(folderprovider.Object);

            //Act
            datastore.AddOrUpdateAsync(new[] { story1, story2 }).Wait();

            //Assert
            storagefolder.Verify(mock => mock.CreateFileForItemAsync(story1, story1.Id.ToString()));
            storagefolder.Verify(mock => mock.CreateFileForItemAsync(story2, story2.Id.ToString()));
        }

        [TestMethod()]
        public void Adding_item_with_the_same_Id_replaces_current_file()
        {
            //Arrange
            var id = 42;
            var story1 = new Story
            {
                Id = id
            };

            var story2 = new Story
            {
                Id = id
            };

            var storagefolder = new Mock<IStorageFolder<Story>>();

            var folderprovider = new Mock<IStorageFolderProvider>();
            folderprovider
                .Setup(fake => fake.GetStorageFolder<Story>(It.IsAny<string>()))
                .Returns(storagefolder.Object);

            var datastore = new InMemoryFileBackedStoryDataStore(folderprovider.Object);

            //Act
            datastore.AddOrUpdateAsync(new[] { story1 }).Wait();
            datastore.AddOrUpdateAsync(new[] { story2 }).Wait();

            //Assert
            storagefolder.Verify(mock => mock.ReplaceFileWithItemAsync(id.ToString(), story2));
        }

        [TestMethod()]
        public void Update_updates_the_existing_item()
        {
            //Arrange
            var storagefolder = new Mock<IStorageFolder<Story>>();

            var folderprovider = new Mock<IStorageFolderProvider>();
            folderprovider
                .Setup(fake => fake.GetStorageFolder<Story>(It.IsAny<string>()))
                .Returns(storagefolder.Object);

            var datastore = new InMemoryFileBackedStoryDataStore(folderprovider.Object);

            var id = 42;
            var story1 = new Story
            {
                Id = id
            };

            var story2 = new Story
            {
                Id = id
            };
            datastore.AddOrUpdateAsync(new[] { story1 }).Wait();

            //Act
            datastore.UpdateAsync(id, story2).Wait();
            var retrieved = datastore.GetAllAsync().Result;

            //Assert
            Assert.AreEqual(story2, retrieved.Single(item => item.Id == id));
        }

        [TestMethod()]
        public void Trying_to_update_non_existing_item_throws_InvalidOperationException()
        {
            //Arrange
            var storagefolder = new Mock<IStorageFolder<Story>>();

            var folderprovider = new Mock<IStorageFolderProvider>();
            folderprovider
                .Setup(fake => fake.GetStorageFolder<Story>(It.IsAny<string>()))
                .Returns(storagefolder.Object);

            var datastore = new InMemoryFileBackedStoryDataStore(folderprovider.Object);

            var id = 42;
            var story1 = new Story
            {
                Id = id
            };


            //Act &&
            //Assert
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => datastore.UpdateAsync(id, story1)).Wait();
        }

        [TestMethod()]
        public void Trying_to_set_an_item_with_id_that_differs_from_supplied_id_throws_InvalidOperationException()
        {
            //Arrange
            var storagefolder = new Mock<IStorageFolder<Story>>();

            var folderprovider = new Mock<IStorageFolderProvider>();
            folderprovider
                .Setup(fake => fake.GetStorageFolder<Story>(It.IsAny<string>()))
                .Returns(storagefolder.Object);

            var datastore = new InMemoryFileBackedStoryDataStore(folderprovider.Object);

            var id = 42;
            var story1 = new Story
            {
                Id = id
            };
            datastore.AddOrUpdateAsync(new[] { story1 }).Wait(); //first, the id must exist

            var story2 = new Story
            {
                Id = 314
            };

            //Act &&
            //Assert
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => datastore.UpdateAsync(id, story2)).Wait();
        }

        [TestMethod()]
        public void Updating_item_updates_corresponding_file()
        {
            //Arrange
            var id = 42;
            var story1 = new Story
            {
                Id = id
            };

            var story2 = new Story
            {
                Id = id
            };

            var storagefolder = new Mock<IStorageFolder<Story>>();

            var folderprovider = new Mock<IStorageFolderProvider>();
            folderprovider
                .Setup(fake => fake.GetStorageFolder<Story>(It.IsAny<string>()))
                .Returns(storagefolder.Object);

            var datastore = new InMemoryFileBackedStoryDataStore(folderprovider.Object);

            datastore.AddOrUpdateAsync(new[] { story1 }).Wait();
            //Act
            datastore.UpdateAsync(id, story2).Wait();

            //Assert
            storagefolder.Verify(mock => mock.ReplaceFileWithItemAsync(id.ToString(), story2));
        }

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