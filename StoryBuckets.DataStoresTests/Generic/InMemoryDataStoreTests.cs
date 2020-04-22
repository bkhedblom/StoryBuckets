using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StoryBuckets.DataStores.Generic;
using StoryBuckets.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StoryBuckets.DataStores.Generic.Tests
{
    [TestClass()]
    public class InMemoryDataStoreTests
    {
        [TestMethod()]
        public void Store_Starts_out_empty()
        {
            //Arrange
            var datastore = new InMemoryDataStore<IData>();

            //Act
            //Assert
            Assert.IsTrue(datastore.IsEmpty);
        }

        [TestMethod()]
        public void After_adding_items_store_is_no_longer_emtpy()
        {
            //Arrange
            var datastore = new InMemoryDataStore<IData>();

            var items = new[]
            {
                new Mock<IData>().Object
            };

            //Act
            datastore.AddOrUpdateAsync(items).Wait();

            //Assert
            Assert.IsFalse(datastore.IsEmpty);
        }

        [TestMethod()]
        public void Added_items_can_be_retrieved()
        {
            //Arrange
            var datastore = new InMemoryDataStore<IData>();

            var items = new[]
            {
                new Mock<IData>().Object
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
            var datastore = new InMemoryDataStore<IData>();

            var items = new[]
            {
                new Mock<IData>().Object
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
            var datastore = new InMemoryDataStore<IData>();

            var id = 42;
            var item1 = new Mock<IData>();
            item1.SetupGet(fake => fake.Id).Returns(id);

            var item2 = new Mock<IData>();
            item2.SetupGet(fake => fake.Id).Returns(id);

            var items = new[]
            {
                item1.Object,
                item2.Object
            };

            //Act
            datastore.AddOrUpdateAsync(items).Wait();
            var retrieved = datastore.GetAllAsync().Result;

            //Assert
            Assert.AreEqual(1, retrieved.Count(item => item.Id == id));
        }

        [TestMethod()]
        public void Adding_the_same_Id_again_updates_exising_item()
        {
            //Arrange
            var datastore = new InMemoryDataStore<IData>();

            var id = 42;
            var item1 = new Mock<IData>();
            item1.SetupGet(fake => fake.Id).Returns(id);

            var item2 = new Mock<IData>();
            item2.SetupGet(fake => fake.Id).Returns(id);

            //Act
            datastore.AddOrUpdateAsync(new[] { item1.Object }).Wait();
            datastore.AddOrUpdateAsync(new[] { item2.Object }).Wait();
            var retrieved = datastore.GetAllAsync().Result;

            //Assert
            Assert.AreEqual(item2.Object, retrieved.Single(item => item.Id == id));
        }

        [TestMethod()]
        public void Update_updated_the_existing_item()
        {
            //Arrange
            var datastore = new InMemoryDataStore<IData>();

            var id = 42;
            var item1 = new Mock<IData>();
            item1.SetupGet(fake => fake.Id).Returns(id);

            var item2 = new Mock<IData>();
            item2.SetupGet(fake => fake.Id).Returns(id);
            datastore.AddOrUpdateAsync(new[] { item1.Object }).Wait();

            //Act
            datastore.UpdateAsync(id, item2.Object).Wait();
            var retrieved = datastore.GetAllAsync().Result;

            //Assert
            Assert.AreEqual(item2.Object, retrieved.Single(item => item.Id == id));
        }

        [TestMethod()]
        public void Trying_to_update_non_existing_item_throws_InvalidOperationException()
        {
            //Arrange
            var datastore = new InMemoryDataStore<IData>();

            var id = 42;
            var item1 = new Mock<IData>();
            item1.SetupGet(fake => fake.Id).Returns(id);


            //Act &&
            //Assert
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => datastore.UpdateAsync(id, item1.Object)).Wait();
        }

        [TestMethod()]
        public void Trying_to_set_an_item_with_id_that_differs_from_supplied_id_throws_InvalidOperationException()
        {
            //Arrange
            var datastore = new InMemoryDataStore<IData>();

            var id = 42;
            var item1 = new Mock<IData>();
            item1.SetupGet(fake => fake.Id).Returns(id);
            datastore.AddOrUpdateAsync(new[] { item1.Object }).Wait(); //first, the id must exist

            var item2 = new Mock<IData>();
            item2.SetupGet(fake => fake.Id).Returns(314);

            //Act &&
            //Assert
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => datastore.UpdateAsync(id, item2.Object)).Wait();
        }
    }
}