using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StoryBuckets.DataStores.FileStorage;
using StoryBuckets.DataStores.FileStore;
using StoryBuckets.DataStores.Stories.Model;
using StoryBuckets.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StoryBuckets.DataStores.Stories.Tests
{
    [TestClass()]
    public class InMemoryFileBackedStoryDataStoreTests
    {
        [TestMethod()]
        public void Added_items_can_be_retrieved()
        {
            //Arrange
            var storagefolder = new Mock<IStorageFolder<FileStoredStory>>();

            var folderprovider = new Mock<IStorageFolderProvider>();
            folderprovider
                .Setup(fake => fake.GetStorageFolder<FileStoredStory>(It.IsAny<string>()))
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
            var storagefolder = new Mock<IStorageFolder<FileStoredStory>>();

            var folderprovider = new Mock<IStorageFolderProvider>();
            folderprovider
                .Setup(fake => fake.GetStorageFolder<FileStoredStory>(It.IsAny<string>()))
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
            var storagefolder = new Mock<IStorageFolder<FileStoredStory>>();

            var folderprovider = new Mock<IStorageFolderProvider>();
            folderprovider
                .Setup(fake => fake.GetStorageFolder<FileStoredStory>(It.IsAny<string>()))
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
            var storagefolder = new Mock<IStorageFolder<FileStoredStory>>();

            var folderprovider = new Mock<IStorageFolderProvider>();
            folderprovider
                .Setup(fake => fake.GetStorageFolder<FileStoredStory>(It.IsAny<string>()))
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
            var storagefolder = new Mock<IStorageFolder<FileStoredStory>>();
            storagefolder
                .Setup(fake => fake.GetStoredItemsAsync())
                .Returns(MakeAsync(new List<FileStoredStory>()));

            var folderprovider = new Mock<IStorageFolderProvider>();
            folderprovider
                .Setup(fake => fake.GetStorageFolder<FileStoredStory>(It.IsAny<string>()))
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
            folderprovider.Verify(mock => mock.GetStorageFolder<FileStoredStory>("stories"));
        }

        [TestMethod()]
        public void Initializing_gets_the_items_from_StorageFolder()
        {
            //Arrange
            var storagefolder = new Mock<IStorageFolder<FileStoredStory>>();
            storagefolder
                .Setup(fake => fake.GetStoredItemsAsync())
                .Returns(MakeAsync(new List<FileStoredStory>()));

            var folderprovider = new Mock<IStorageFolderProvider>();
            folderprovider
                .Setup(fake => fake.GetStorageFolder<FileStoredStory>(It.IsAny<string>()))
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
            var story = new FileStoredStory
            {
                Id = 42
            };

            var storagefolder = new Mock<IStorageFolder<FileStoredStory>>();
            storagefolder
                .Setup(fake => fake.GetStoredItemsAsync())
                .Returns(MakeAsync(new[] { story }));

            var folderprovider = new Mock<IStorageFolderProvider>();
            folderprovider
                .Setup(fake => fake.GetStorageFolder<FileStoredStory>(It.IsAny<string>()))
                .Returns(storagefolder.Object);

            var datastore = new InMemoryFileBackedStoryDataStore(folderprovider.Object);

            //Act
            datastore.InitializeAsync().Wait();

            //Assert
            var result = datastore.GetAllAsync().Result;
            Assert.AreEqual(story.Id, result.Single().Id);
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

            var storagefolder = new Mock<IStorageFolder<FileStoredStory>>();

            var folderprovider = new Mock<IStorageFolderProvider>();
            folderprovider
                .Setup(fake => fake.GetStorageFolder<FileStoredStory>(It.IsAny<string>()))
                .Returns(storagefolder.Object);

            var datastore = new InMemoryFileBackedStoryDataStore(folderprovider.Object);

            //Act
            datastore.AddOrUpdateAsync(new[] { story1, story2 }).Wait();

            //Assert
            storagefolder.Verify(mock => mock.CreateFileForItemAsync(It.IsAny<FileStoredStory>(), story1.Id.ToString()));
            storagefolder.Verify(mock => mock.CreateFileForItemAsync(It.IsAny<FileStoredStory>(), story2.Id.ToString()));
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

            var storagefolder = new Mock<IStorageFolder<FileStoredStory>>();

            var folderprovider = new Mock<IStorageFolderProvider>();
            folderprovider
                .Setup(fake => fake.GetStorageFolder<FileStoredStory>(It.IsAny<string>()))
                .Returns(storagefolder.Object);

            var datastore = new InMemoryFileBackedStoryDataStore(folderprovider.Object);

            //Act
            datastore.AddOrUpdateAsync(new[] { story1 }).Wait();
            datastore.AddOrUpdateAsync(new[] { story2 }).Wait();

            //Assert
            storagefolder.Verify(mock => mock.ReplaceFileWithItemAsync(id.ToString(), It.IsAny<FileStoredStory>()));
        }

        [TestMethod()]
        public void Update_updates_the_existing_item()
        {
            //Arrange
            var storagefolder = new Mock<IStorageFolder<FileStoredStory>>();

            var folderprovider = new Mock<IStorageFolderProvider>();
            folderprovider
                .Setup(fake => fake.GetStorageFolder<FileStoredStory>(It.IsAny<string>()))
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
            var storagefolder = new Mock<IStorageFolder<FileStoredStory>>();

            var folderprovider = new Mock<IStorageFolderProvider>();
            folderprovider
                .Setup(fake => fake.GetStorageFolder<FileStoredStory>(It.IsAny<string>()))
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
            var storagefolder = new Mock<IStorageFolder<FileStoredStory>>();

            var folderprovider = new Mock<IStorageFolderProvider>();
            folderprovider
                .Setup(fake => fake.GetStorageFolder<FileStoredStory>(It.IsAny<string>()))
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

            var storagefolder = new Mock<IStorageFolder<FileStoredStory>>();

            var folderprovider = new Mock<IStorageFolderProvider>();
            folderprovider
                .Setup(fake => fake.GetStorageFolder<FileStoredStory>(It.IsAny<string>()))
                .Returns(storagefolder.Object);

            var datastore = new InMemoryFileBackedStoryDataStore(folderprovider.Object);

            datastore.AddOrUpdateAsync(new[] { story1 }).Wait();
            //Act
            datastore.UpdateAsync(id, story2).Wait();

            //Assert
            storagefolder.Verify(mock => mock.ReplaceFileWithItemAsync(id.ToString(), It.IsAny<FileStoredStory>()));
        }

        [TestMethod()]
        public void Get_retrieves_stories_with_requested_ids()
        {
            //Arrange
            var id1 = 42;
            var id2 = 27;

            var story1 = new FileStoredStory
            {
                Id = id1
            };

            var story2 = new FileStoredStory
            {
                Id = id2
            };

            var story3 = new FileStoredStory
            {
                Id = 123
            };

            var storagefolder = new Mock<IStorageFolder<FileStoredStory>>();
            storagefolder
                .Setup(fake => fake.GetStoredItemsAsync())
                .Returns(MakeAsync(new[] { story1, story2, story3 }));

            var folderprovider = new Mock<IStorageFolderProvider>();
            folderprovider
                .Setup(fake => fake.GetStorageFolder<FileStoredStory>(It.IsAny<string>()))
                .Returns(storagefolder.Object);

            var datastore = new InMemoryFileBackedStoryDataStore(folderprovider.Object);

            datastore.InitializeAsync().Wait();

            //Act
            var result = datastore.GetAsync(new[] { id1, id2 }).Result;

            //Assert
            var containsStory1 = result.Any(b => b.Id == id1);
            var containsStory2 = result.Any(b => b.Id == id2);
            Assert.IsTrue(containsStory1);
            Assert.IsTrue(containsStory2);
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod()]
        public async Task Should_not_load_items_if_another_initialization_is_in_progressAsync()
        {
            //Arrange
            var storagefolder = new Mock<IStorageFolder<FileStoredStory>>();
            var testHelper = new AsyncReadTester();
            storagefolder
                .Setup(fake => fake.GetStoredItemsAsync())
                .Returns(testHelper.ReadAsync());

            var folderprovider = new Mock<IStorageFolderProvider>();
            folderprovider
                .Setup(fake => fake.GetStorageFolder<FileStoredStory>(It.IsAny<string>()))
                .Returns(storagefolder.Object);


            var datastore = new InMemoryFileBackedStoryDataStore(folderprovider.Object);

            var firstInit = datastore.InitializeAsync();

            //Act
            var secondInit = datastore.InitializeAsync();

            testHelper.StopFakeReading();

            await Task.WhenAll(firstInit, secondInit);

            //Assert
            Assert.IsFalse(testHelper.CalledWhileInProgress);
        }

        [TestMethod()]
        public async Task Should_not_try_to_write_files_if_another_instance_is_reading()
        {
            //Arrange
            var storagefolder = new Mock<IStorageFolder<FileStoredStory>>();
            var testHelper = new AsyncReadTester();
            storagefolder
                .Setup(mock => mock.GetStoredItemsAsync())
                .Returns(testHelper.ReadAsync());
            storagefolder
                .Setup(mock => mock.CreateFileForItemAsync(It.IsAny<FileStoredStory>(), It.IsAny<string>()))
                .Callback(() => testHelper.SetOtherFunctionCalled());

            var folderprovider = new Mock<IStorageFolderProvider>();
            folderprovider
                .Setup(fake => fake.GetStorageFolder<FileStoredStory>(It.IsAny<string>()))
                .Returns(storagefolder.Object);


            var datastore1 = new InMemoryFileBackedStoryDataStore(folderprovider.Object);
            var datastore2 = new InMemoryFileBackedStoryDataStore(folderprovider.Object);

            var reading = datastore1.InitializeAsync();

            //Act
            var writing = datastore2.AddOrUpdateAsync(new[] { new Story() });

            testHelper.StopFakeReading();

            await Task.WhenAll(reading, writing);

            //Assert
            Assert.IsFalse(testHelper.CalledWhileInProgress);
        }

        private class AsyncReadTester
        {
            private bool _continueFakeReading = true;
            private bool _inProgress;

            public bool CalledWhileInProgress { get; private set; }

            public void StopFakeReading()
            {
                _continueFakeReading = false;
            }

            public async IAsyncEnumerable<FileStoredStory> ReadAsync()
            {
                CalledWhileInProgress = _inProgress;
                _inProgress = true;
                do
                {
                    await Task.Delay(1);
                    yield return new FileStoredStory();
                } while (_continueFakeReading);
                _inProgress = false;
            }

            public Task SetOtherFunctionCalled()
            {
                CalledWhileInProgress = _inProgress;
                return Task.CompletedTask;
            }

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