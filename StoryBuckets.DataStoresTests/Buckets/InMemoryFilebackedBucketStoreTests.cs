using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StoryBuckets.DataStores;
using StoryBuckets.DataStores.Buckets;
using StoryBuckets.DataStores.Buckets.Model;
using StoryBuckets.DataStores.FileStorage;
using StoryBuckets.DataStores.FileStore;
using StoryBuckets.DataStores.Stories;
using StoryBuckets.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
            var storyStore = new Mock<IDataStore<Story>>();

            //Act
            _ = new InMemoryFileBackedBucketDataStore(folderprovider.Object, storyStore.Object);

            //Assert
            folderprovider.Verify(mock => mock.GetStorageFolder<FileStoredBucket>("buckets"));
        }

        [TestMethod()]
        public void IsInitialized_starts_out_false()
        {
            //Arrange
            var folderprovider = new Mock<IStorageFolderProvider>();
            var storyStore = new Mock<IDataStore<Story>>();

            var datastore = new InMemoryFileBackedBucketDataStore(folderprovider.Object, storyStore.Object);

            //Act
            //Assert
            Assert.IsFalse(datastore.IsInitialized);
        }

        [TestMethod()]
        public async Task IsInitialized_is_true_after_calling_initializeAsync()
        {
            //Arrange
            var storagefolder = new Mock<IStorageFolder<FileStoredBucket>>();
            storagefolder
                .Setup(fake => fake.GetStoredItemsAsync())
                .Returns(MakeAsync(new List<FileStoredBucket>()));

            var folderprovider = new Mock<IStorageFolderProvider>();
            folderprovider
                .Setup(fake => fake.GetStorageFolder<FileStoredBucket>(It.IsAny<string>()))
                .Returns(storagefolder.Object);

            var storyStore = new Mock<IDataStore<Story>>();

            var datastore = new InMemoryFileBackedBucketDataStore(folderprovider.Object, storyStore.Object);

            //Act
            await datastore.InitializeAsync();

            //Assert
            Assert.IsTrue(datastore.IsInitialized);
        }

        [TestMethod()]
        public async Task Initializing_gets_the_items_from_StorageFolderAsync()
        {
            //Arrange
            var storagefolder = new Mock<IStorageFolder<FileStoredBucket>>();
            storagefolder
                .Setup(fake => fake.GetStoredItemsAsync())
                .Returns(MakeAsync(new List<FileStoredBucket>()));

            var folderprovider = new Mock<IStorageFolderProvider>();
            folderprovider
                .Setup(fake => fake.GetStorageFolder<FileStoredBucket>(It.IsAny<string>()))
                .Returns(storagefolder.Object);

            var storyStore = new Mock<IDataStore<Story>>();

            var datastore = new InMemoryFileBackedBucketDataStore(folderprovider.Object, storyStore.Object);

            //Act
            await datastore.InitializeAsync();

            //Assert
            storagefolder.Verify(mock => mock.GetStoredItemsAsync(), Times.Once);
        }

        [TestMethod()]
        public async Task Initialize_stores_the_retrieved_items_in_the_DataStoreAsync()
        {
            //Arrange
            var bucket = new FileStoredBucket
            {
                Id = 42
            };

            var storagefolder = new Mock<IStorageFolder<FileStoredBucket>>();
            storagefolder
                .Setup(fake => fake.GetStoredItemsAsync())
                .Returns(MakeAsync(new[] { bucket }));

            var folderprovider = new Mock<IStorageFolderProvider>();
            folderprovider
                .Setup(fake => fake.GetStorageFolder<FileStoredBucket>(It.IsAny<string>()))
                .Returns(storagefolder.Object);

            var storyStore = new Mock<IDataStore<Story>>();

            var datastore = new InMemoryFileBackedBucketDataStore(folderprovider.Object, storyStore.Object);

            //Act
            await datastore.InitializeAsync();

            //Assert
            var result = datastore.GetAllAsync().Result;
            Assert.AreEqual(bucket.Id, result.Single().Id);
        }

        [TestMethod()]
        public async Task Initializing_initializes_StoryStoreAsync()
        {
            //Arrange
            var storagefolder = new Mock<IStorageFolder<FileStoredBucket>>();
            storagefolder
                .Setup(fake => fake.GetStoredItemsAsync())
                .Returns(MakeAsync(new List<FileStoredBucket>()));

            var folderprovider = new Mock<IStorageFolderProvider>();
            folderprovider
                .Setup(fake => fake.GetStorageFolder<FileStoredBucket>(It.IsAny<string>()))
                .Returns(storagefolder.Object);

            var storyStore = new Mock<IDataStore<Story>>();

            var datastore = new InMemoryFileBackedBucketDataStore(folderprovider.Object, storyStore.Object);

            //Act
            await datastore.InitializeAsync();

            //Assert
            storyStore.Verify(mock => mock.InitializeAsync(), Times.Once);
        }

        [TestMethod()]
        public async Task Initializing_do_not_initialize_already_initialized_StoryStoreAsync()
        {
            //Arrange
            var storagefolder = new Mock<IStorageFolder<FileStoredBucket>>();
            storagefolder
                .Setup(fake => fake.GetStoredItemsAsync())
                .Returns(MakeAsync(new List<FileStoredBucket>()));

            var folderprovider = new Mock<IStorageFolderProvider>();
            folderprovider
                .Setup(fake => fake.GetStorageFolder<FileStoredBucket>(It.IsAny<string>()))
                .Returns(storagefolder.Object);

            var storyStore = new Mock<IDataStore<Story>>();
            storyStore
                .SetupGet(fake => fake.IsInitialized)
                .Returns(true);

            var datastore = new InMemoryFileBackedBucketDataStore(folderprovider.Object, storyStore.Object);

            //Act
            await datastore.InitializeAsync();

            //Assert
            storyStore.Verify(mock => mock.InitializeAsync(), Times.Never);
        }

        [TestMethod()]
        public async Task Initializing_waits_for_StoryStory_to_initialize_before_starting_to_read_stories()
        { //Since the story store will be called during the conversion of stored buckets
            //Arrange
            var readCalledDuringStoryStoreInit = false;
            var storyStoreInitCompletionSource = new TaskCompletionSource<bool>(); //TResult must be given, bool as recommended as placeholder
            var storagefolder = new Mock<IStorageFolder<FileStoredBucket>>();
            storagefolder
                .Setup(fake => fake.GetStoredItemsAsync())
                .Returns(MakeAsync(new List<FileStoredBucket>()))
                .Callback(() => 
                    readCalledDuringStoryStoreInit = !storyStoreInitCompletionSource.Task.IsCompleted);

            var folderprovider = new Mock<IStorageFolderProvider>();
            folderprovider
                .Setup(fake => fake.GetStorageFolder<FileStoredBucket>(It.IsAny<string>()))
                .Returns(storagefolder.Object);

            var storyStore = new Mock<IDataStore<Story>>();
            storyStore
                .Setup(mock => mock.InitializeAsync())
                .Returns(storyStoreInitCompletionSource.Task);

            var datastore = new InMemoryFileBackedBucketDataStore(folderprovider.Object, storyStore.Object);

            //Act
            var initing = datastore.InitializeAsync();
            
            storyStoreInitCompletionSource.SetResult(true);
            await initing;

            //Assert
            storyStore.Verify(mock => mock.InitializeAsync(), Times.Once);
            Assert.IsFalse(readCalledDuringStoryStoreInit);
        }

        [TestMethod()]
        public async Task Get_retrieves_buckets_with_requested_idsAsync()
        {
            //Arrange
            var id1 = 42;
            var id2 = 27;

            var bucket1 = new FileStoredBucket
            {
                Id = id1
            };

            var bucket2 = new FileStoredBucket
            {
                Id = id2
            };

            var bucket3 = new FileStoredBucket
            {
                Id = 123
            };

            var storagefolder = new Mock<IStorageFolder<FileStoredBucket>>();
            storagefolder
                .Setup(fake => fake.GetStoredItemsAsync())
                .Returns(MakeAsync(new[] { bucket1, bucket2, bucket3 }));

            var folderprovider = new Mock<IStorageFolderProvider>();
            folderprovider
                .Setup(fake => fake.GetStorageFolder<FileStoredBucket>(It.IsAny<string>()))
                .Returns(storagefolder.Object);

            var storyStore = new Mock<IDataStore<Story>>();

            var datastore = new InMemoryFileBackedBucketDataStore(folderprovider.Object, storyStore.Object);
            
            await datastore.InitializeAsync();

            //Act
            var result = datastore.GetAsync(new[] { id1, id2 }).Result;

            //Assert
            var containsBucket1 = result.Any(b => b.Id == id1);
            var containsBucket2 = result.Any(b => b.Id == id2);
            Assert.IsTrue(containsBucket1);
            Assert.IsTrue(containsBucket2);
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod()]
        public async Task Stories_are_fetched_from_StoryStoreAsync()
        {
            //Arrange
            var containedStoryIds = new[] { 31415, 2718 };
            var bucket = new FileStoredBucket
            {
                Id = 42,
                StoryIds = containedStoryIds.AsEnumerable()
            };

            var storagefolder = new Mock<IStorageFolder<FileStoredBucket>>();
            storagefolder
                .Setup(fake => fake.GetStoredItemsAsync())
                .Returns(MakeAsync(new[] { bucket }));

            var folderprovider = new Mock<IStorageFolderProvider>();
            folderprovider
                .Setup(fake => fake.GetStorageFolder<FileStoredBucket>(It.IsAny<string>()))
                .Returns(storagefolder.Object);

            var storyStore = new Mock<IDataStore<Story>>();

            var datastore = new InMemoryFileBackedBucketDataStore(folderprovider.Object, storyStore.Object);


            //Act
            await datastore.InitializeAsync();
            _ = await datastore.GetAllAsync();

            //Assert
            storyStore.Verify(mock => mock.GetAsync(containedStoryIds), Times.Once);
        }

        [TestMethod()]
        public async Task Contained_stories_from_StoryStore_are_added_to_BucketAsync()
        {
            //Arrange
            var id = 42;
            var bucket = new FileStoredBucket
            {
                Id = id
            };

            var storagefolder = new Mock<IStorageFolder<FileStoredBucket>>();
            storagefolder
                .Setup(fake => fake.GetStoredItemsAsync())
                .Returns(MakeAsync(new[] { bucket }));

            var folderprovider = new Mock<IStorageFolderProvider>();
            folderprovider
                .Setup(fake => fake.GetStorageFolder<FileStoredBucket>(It.IsAny<string>()))
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
            var storyStore = new Mock<IDataStore<Story>>();
            storyStore
                .Setup(fake => fake.GetAsync(It.IsAny<IEnumerable<int>>()))
                .ReturnsAsync(stories);

            var datastore = new InMemoryFileBackedBucketDataStore(folderprovider.Object, storyStore.Object);

            //Act
            await datastore.InitializeAsync();
            var buckets = await datastore.GetAllAsync();

            //Assert
            var storiesOfBucket = buckets.Single().Stories;
            Assert.AreEqual(stories.Length, storiesOfBucket.Count);
            foreach (var fakeStory in stories)
            {
                var bucketContainsStory = storiesOfBucket.Any(returnedStory => returnedStory.Id == fakeStory.Id);
                Assert.IsTrue(bucketContainsStory, $"Story with id {fakeStory.Id} not found in bucket!");
            }
        }

        [TestMethod()]
        public async Task Added_items_can_be_retrievedAsync()
        {
            //Arrange
            var storagefolder = new Mock<IStorageFolder<FileStoredBucket>>();

            var folderprovider = new Mock<IStorageFolderProvider>();
            folderprovider
                .Setup(fake => fake.GetStorageFolder<FileStoredBucket>(It.IsAny<string>()))
                .Returns(storagefolder.Object);

            var storyStore = new Mock<IDataStore<Story>>();

            var datastore = new InMemoryFileBackedBucketDataStore(folderprovider.Object, storyStore.Object);

            var items = new[]
            {
                new Bucket()
            };

            //Act
            await datastore.AddOrUpdateAsync(items);
            var retrieved = await datastore.GetAllAsync();

            //Assert
            Assert.AreEqual(items.Single(), retrieved.Single());
        }

        [TestMethod()]
        public async Task The_same_item_is_not_added_twiceAsync()
        {
            //Arrange
            var storagefolder = new Mock<IStorageFolder<FileStoredBucket>>();

            var folderprovider = new Mock<IStorageFolderProvider>();
            folderprovider
                .Setup(fake => fake.GetStorageFolder<FileStoredBucket>(It.IsAny<string>()))
                .Returns(storagefolder.Object);

            var storyStore = new Mock<IDataStore<Story>>();

            var datastore = new InMemoryFileBackedBucketDataStore(folderprovider.Object, storyStore.Object);

            var items = new[]
            {
                new Bucket()
            };

            //Act
            await datastore.AddOrUpdateAsync(items);
            var countAfterFirstAdd = (await datastore.GetAllAsync()).Count();

            datastore.AddOrUpdateAsync(items).Wait();
            var countAfterSecondAdd = (await datastore.GetAllAsync()).Count();

            //Assert
            Assert.AreEqual(countAfterFirstAdd, countAfterSecondAdd);
        }

        [TestMethod()]
        public async Task The_same_Id_exists_only_onceAsync()
        {
            //Arrange
            var storagefolder = new Mock<IStorageFolder<FileStoredBucket>>();

            var folderprovider = new Mock<IStorageFolderProvider>();
            folderprovider
                .Setup(fake => fake.GetStorageFolder<FileStoredBucket>(It.IsAny<string>()))
                .Returns(storagefolder.Object);

            var storyStore = new Mock<IDataStore<Story>>();

            var datastore = new InMemoryFileBackedBucketDataStore(folderprovider.Object, storyStore.Object);

            var id = 42;

            var items = new[]
            {
                new Bucket
                {
                    Id = id
                },
                new Bucket
                {
                    Id = id
                }
            };

            //Act
            await datastore.AddOrUpdateAsync(items);
            var retrieved = await datastore.GetAllAsync();

            //Assert
            Assert.AreEqual(1, retrieved.Count(item => item.Id == id));
        }

        [TestMethod()]
        public async Task Adding_the_same_Id_again_updates_exising_item_to_the_newAsync()
        {
            //Arrange
            var storagefolder = new Mock<IStorageFolder<FileStoredBucket>>();

            var folderprovider = new Mock<IStorageFolderProvider>();
            folderprovider
                .Setup(fake => fake.GetStorageFolder<FileStoredBucket>(It.IsAny<string>()))
                .Returns(storagefolder.Object);

            var storyStore = new Mock<IDataStore<Story>>();

            var datastore = new InMemoryFileBackedBucketDataStore(folderprovider.Object, storyStore.Object);

            var id = 42;
            var item1 = new Bucket
            {
                Id = id
            };

            var item2 = new Bucket
            {
                Id = id
            };

            //Act
            await datastore.AddOrUpdateAsync(new[] { item1 });
            await datastore.AddOrUpdateAsync(new[] { item2 });
            var retrieved = await datastore.GetAllAsync();

            //Assert
            Assert.AreEqual(item2, retrieved.Single(item => item.Id == id));
        }

        [TestMethod()]
        public async Task Adding_items_creates_new_files_for_them_using_id_as_FilenameAsync()
        {
            //Arrange
            var bucket1 = new Bucket
            {
                Id = 42
            };

            var bucket2 = new Bucket
            {
                Id = 314
            };

            var storagefolder = new Mock<IStorageFolder<FileStoredBucket>>();

            var folderprovider = new Mock<IStorageFolderProvider>();
            folderprovider
                .Setup(fake => fake.GetStorageFolder<FileStoredBucket>(It.IsAny<string>()))
                .Returns(storagefolder.Object);

            var storyStore = new Mock<IDataStore<Story>>();

            var datastore = new InMemoryFileBackedBucketDataStore(folderprovider.Object, storyStore.Object);

            //Act
            await datastore.AddOrUpdateAsync(new[] { bucket1, bucket2 });

            //Assert
            storagefolder.Verify(mock => mock.CreateFileForItemAsync(It.IsAny<FileStoredBucket>(), bucket1.Id.ToString()));
            storagefolder.Verify(mock => mock.CreateFileForItemAsync(It.IsAny<FileStoredBucket>(), bucket2.Id.ToString()));
        }

        [TestMethod()]
        public async Task Adding_item_with_the_same_Id_replaces_current_fileAsync()
        {
            //Arrange
            var id = 42;
            var bucket1 = new Bucket
            {
                Id = id
            };

            var bucket2 = new Bucket
            {
                Id = id
            };

            var storagefolder = new Mock<IStorageFolder<FileStoredBucket>>();

            var folderprovider = new Mock<IStorageFolderProvider>();
            folderprovider
                .Setup(fake => fake.GetStorageFolder<FileStoredBucket>(It.IsAny<string>()))
                .Returns(storagefolder.Object);

            var storyStore = new Mock<IDataStore<Story>>();

            var datastore = new InMemoryFileBackedBucketDataStore(folderprovider.Object, storyStore.Object);

            //Act
            await datastore.AddOrUpdateAsync(new[] { bucket1 });
            await datastore.AddOrUpdateAsync(new[] { bucket2 });

            //Assert
            storagefolder.Verify(mock => mock.ReplaceFileWithItemAsync(id.ToString(), It.IsAny<FileStoredBucket>()));
        }

        [TestMethod()]
        public async Task Update_updates_the_existing_itemAsync()
        {
            //Arrange
            var storagefolder = new Mock<IStorageFolder<FileStoredBucket>>();

            var folderprovider = new Mock<IStorageFolderProvider>();
            folderprovider
                .Setup(fake => fake.GetStorageFolder<FileStoredBucket>(It.IsAny<string>()))
                .Returns(storagefolder.Object);

            var storyStore = new Mock<IDataStore<Story>>();

            var datastore = new InMemoryFileBackedBucketDataStore(folderprovider.Object, storyStore.Object);

            var id = 42;
            var bucket1 = new Bucket
            {
                Id = id
            };

            var bucket2 = new Bucket
            {
                Id = id
            };
            datastore.AddOrUpdateAsync(new[] { bucket1 }).Wait();

            //Act
            await datastore.UpdateAsync(id, bucket2);
            var retrieved = await datastore.GetAllAsync();

            //Assert
            Assert.AreEqual(bucket2, retrieved.Single(item => item.Id == id));
        }

        [TestMethod()]
        public async Task Trying_to_update_non_existing_item_throws_InvalidOperationExceptionAsync()
        {
            //Arrange
            var storagefolder = new Mock<IStorageFolder<FileStoredBucket>>();

            var folderprovider = new Mock<IStorageFolderProvider>();
            folderprovider
                .Setup(fake => fake.GetStorageFolder<FileStoredBucket>(It.IsAny<string>()))
                .Returns(storagefolder.Object);

            var storyStore = new Mock<IDataStore<Story>>();

            var datastore = new InMemoryFileBackedBucketDataStore(folderprovider.Object, storyStore.Object);

            var id = 42;
            var bucket1 = new Bucket
            {
                Id = id
            };


            //Act &&
            //Assert
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () => await datastore.UpdateAsync(id, bucket1));
        }

        [TestMethod()]
        public async Task Trying_to_set_an_item_with_id_that_differs_from_supplied_id_throws_InvalidOperationExceptionAsync()
        {
            //Arrange
            var storagefolder = new Mock<IStorageFolder<FileStoredBucket>>();

            var folderprovider = new Mock<IStorageFolderProvider>();
            folderprovider
                .Setup(fake => fake.GetStorageFolder<FileStoredBucket>(It.IsAny<string>()))
                .Returns(storagefolder.Object);

            var storyStore = new Mock<IDataStore<Story>>();

            var datastore = new InMemoryFileBackedBucketDataStore(folderprovider.Object, storyStore.Object);

            var id = 42;
            var bucket1 = new Bucket
            {
                Id = id
            };
            await datastore.AddOrUpdateAsync(new[] { bucket1 }); //first, the id must exist

            var bucket2 = new Bucket
            {
                Id = 314
            };

            //Act &&
            //Assert
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () => await datastore.UpdateAsync(id, bucket2));
        }

        [TestMethod()]
        public async Task Updating_item_updates_corresponding_fileAsync()
        {
            //Arrange
            var id = 42;
            var bucket1 = new Bucket
            {
                Id = id
            };

            var bucket2 = new Bucket
            {
                Id = id
            };

            var storagefolder = new Mock<IStorageFolder<FileStoredBucket>>();

            var folderprovider = new Mock<IStorageFolderProvider>();
            folderprovider
                .Setup(fake => fake.GetStorageFolder<FileStoredBucket>(It.IsAny<string>()))
                .Returns(storagefolder.Object);

            var storyStore = new Mock<IDataStore<Story>>();

            var datastore = new InMemoryFileBackedBucketDataStore(folderprovider.Object, storyStore.Object);

            await datastore.AddOrUpdateAsync(new[] { bucket1 });
            //Act
            await datastore.UpdateAsync(id, bucket2);

            //Assert
            storagefolder.Verify(mock => mock.ReplaceFileWithItemAsync(id.ToString(), It.IsAny<FileStoredBucket>()));
        }

        [TestMethod()]
        public async Task Adding_bucket_with_Stories_calls_StoryStore_to_update_stories()
        {
            //Arrange
            var stories = new[]
            {
                new Story()
            };

            var bucket = new Bucket(stories)
            {
                Id = 42
            };

            var storagefolder = new Mock<IStorageFolder<FileStoredBucket>>();

            var folderprovider = new Mock<IStorageFolderProvider>();
            folderprovider
                .Setup(fake => fake.GetStorageFolder<FileStoredBucket>(It.IsAny<string>()))
                .Returns(storagefolder.Object);

            var storyStore = new Mock<IDataStore<Story>>();

            var datastore = new InMemoryFileBackedBucketDataStore(folderprovider.Object, storyStore.Object);

            //Act
            await datastore.AddOrUpdateAsync(new[] { bucket });

            //Assert
            storyStore.Verify(mock => mock.AddOrUpdateAsync(It.IsAny<IEnumerable<Story>>()));
        }

        [TestMethod()]
        public async Task Updating_Bucket_with_stories_calls_StoryStore_to_update_stories()
        {
            //Arrange
            var stories = new[]
            {
                new Story
                {
                    Id = 31415
                },
                new Story
                {
                    Id = 2718
                }
            };

            var id = 42;
            var StoredBucket = new FileStoredBucket
            {
                Id = id
            };

            var updatedBucket = new Bucket(stories)
            {
                Id = id
            };

            var storagefolder = new Mock<IStorageFolder<FileStoredBucket>>();
            storagefolder
                .Setup(fake => fake.GetStoredItemsAsync())
                .Returns(MakeAsync(new[] { StoredBucket }));

            var folderprovider = new Mock<IStorageFolderProvider>();
            folderprovider
                .Setup(fake => fake.GetStorageFolder<FileStoredBucket>(It.IsAny<string>()))
                .Returns(storagefolder.Object);

            var storyStore = new Mock<IDataStore<Story>>();
            
            var datastore = new InMemoryFileBackedBucketDataStore(folderprovider.Object, storyStore.Object);

            await datastore.InitializeAsync();
            
            //Act
            await datastore.UpdateAsync(id, updatedBucket);

            //Assert
            storyStore.Verify(mock => mock.AddOrUpdateAsync(It.IsAny<IEnumerable<Story>>()));
        }

        [TestMethod()]
        public async Task Adding_items_without_Id_gives_them_next_available_Id_and_saves_them_to_that_file()
        {
            //Arrange
            var currentMaxId = 42;
            var earlierStoredBucket = new FileStoredBucket
            {
                Id = currentMaxId - 7
            };
            var lastStoredBucket = new FileStoredBucket
            {
                Id = currentMaxId
            };

            var newBucket1 = new Bucket
            {
                Id = 0
            };

            var newBucket2 = new Bucket
            {
                Id = 0
            };

            var storedBuckets = new Dictionary<string, FileStoredBucket>();

            var storagefolder = new Mock<IStorageFolder<FileStoredBucket>>();
            storagefolder
                .Setup(fake => fake.GetStoredItemsAsync())
                .Returns(MakeAsync(new[] { lastStoredBucket, earlierStoredBucket }));
            storagefolder
                .Setup(mock => mock.CreateFileForItemAsync(It.IsAny<FileStoredBucket>(), It.IsAny<string>()))
                .Callback<FileStoredBucket, string>((bucket, id) => storedBuckets.Add(id, bucket));
            var folderprovider = new Mock<IStorageFolderProvider>();
            folderprovider
                .Setup(fake => fake.GetStorageFolder<FileStoredBucket>(It.IsAny<string>()))
                .Returns(storagefolder.Object);

            var storyStore = new Mock<IDataStore<Story>>();

            var datastore = new InMemoryFileBackedBucketDataStore(folderprovider.Object, storyStore.Object);

            await datastore.InitializeAsync();

            //Act
            await datastore.AddOrUpdateAsync(new[] {  newBucket1, newBucket2 });

            //Assert
            Assert.AreEqual(currentMaxId + 1, newBucket1.Id, "Bucket 1 got wrong Id");
            Assert.AreEqual(currentMaxId + 2, newBucket2.Id, "Bucket 2 got wrong Id");
            Assert.AreEqual(newBucket1.Id, storedBuckets[newBucket1.Id.ToString()].Id, "Bucket 1 did not get the new Id in storage");
            Assert.AreEqual(newBucket2.Id, storedBuckets[newBucket2.Id.ToString()].Id, "Bucket 2 did not get the new Id in storage");
            storagefolder.Verify(mock => mock.CreateFileForItemAsync(It.IsAny<FileStoredBucket>(), newBucket1.Id.ToString()));
            storagefolder.Verify(mock => mock.CreateFileForItemAsync(It.IsAny<FileStoredBucket>(), newBucket2.Id.ToString()));
        }

        [TestMethod()]
        public async Task Updating_Bucket_with_stories_stores_ids_of_those_stories()
        {
            //Arrange
            var stories = new[]
            {
                new Story
                {
                    Id = 31415
                },
                new Story
                {
                    Id = 2718
                }
            };

            var id = 42;
            var StoredBucket = new FileStoredBucket
            {
                Id = id
            };

            var updatedBucket = new Bucket(stories)
            {
                Id = id
            };

            IEnumerable<int> storedStoryIds = null;

            var storagefolder = new Mock<IStorageFolder<FileStoredBucket>>();
            storagefolder
                .Setup(fake => fake.GetStoredItemsAsync())
                .Returns(MakeAsync(new[] { StoredBucket }));
            storagefolder
                .Setup(mock => mock.ReplaceFileWithItemAsync(It.IsAny<string>(), It.IsAny<FileStoredBucket>()))
                .Callback<string, FileStoredBucket>((_, StoredBucket) => storedStoryIds = StoredBucket.StoryIds);

            var folderprovider = new Mock<IStorageFolderProvider>();
            folderprovider
                .Setup(fake => fake.GetStorageFolder<FileStoredBucket>(It.IsAny<string>()))
                .Returns(storagefolder.Object);

            var storyStore = new Mock<IDataStore<Story>>();

            var datastore = new InMemoryFileBackedBucketDataStore(folderprovider.Object, storyStore.Object);

            await datastore.InitializeAsync();

            //Act
            await datastore.UpdateAsync(id, updatedBucket);

            //Assert
            Assert.AreEqual(stories.Count(), storedStoryIds.Count());
            foreach (var story in stories)
            {
                Assert.IsTrue(storedStoryIds.Contains(story.Id), $"Story id ${story.Id} was not stored!");
            }
        }

        [TestMethod()]
        public async Task Should_not_load_items_if_another_initialization_is_in_progress()
        {
            //Arrange
            var storagefolder = new Mock<IStorageFolder<FileStoredBucket>>();
            var testHelper = new AsyncReadTester();
            storagefolder
                .Setup(fake => fake.GetStoredItemsAsync())
                .Returns(testHelper.ReadAsync());

            var folderprovider = new Mock<IStorageFolderProvider>();
            folderprovider
                .Setup(fake => fake.GetStorageFolder<FileStoredBucket>(It.IsAny<string>()))
                .Returns(storagefolder.Object);

            var storyStore = new Mock<IDataStore<Story>>();

            var datastore = new InMemoryFileBackedBucketDataStore(folderprovider.Object, storyStore.Object);

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
            var storagefolder = new Mock<IStorageFolder<FileStoredBucket>>();
            var testHelper = new AsyncReadTester();
            storagefolder
                .Setup(mock => mock.GetStoredItemsAsync())
                .Returns(testHelper.ReadAsync());
            storagefolder
                .Setup(mock => mock.CreateFileForItemAsync(It.IsAny<FileStoredBucket>(), It.IsAny<string>()))
                .Callback(() => testHelper.SetOtherFunctionCalled());

            var folderprovider = new Mock<IStorageFolderProvider>();
            folderprovider
                .Setup(fake => fake.GetStorageFolder<FileStoredBucket>(It.IsAny<string>()))
                .Returns(storagefolder.Object);

            var storyStore = new Mock<IDataStore<Story>>();

            var datastore1 = new InMemoryFileBackedBucketDataStore(folderprovider.Object, storyStore.Object);
            var datastore2 = new InMemoryFileBackedBucketDataStore(folderprovider.Object, storyStore.Object);

            Task reading, writing;
            try 
            { 
                reading = datastore1.InitializeAsync();

            //Act
                writing = datastore2.AddOrUpdateAsync(new[] { new Bucket() });
            }
            finally
            {
                testHelper.StopFakeReading();
            }

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

            public async IAsyncEnumerable<FileStoredBucket> ReadAsync()
            {
                CalledWhileInProgress = _inProgress;
                _inProgress = true;
                do
                {
                    await Task.Delay(1);
                    yield return new FileStoredBucket();
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