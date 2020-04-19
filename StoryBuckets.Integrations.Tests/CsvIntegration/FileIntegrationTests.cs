using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;

namespace StoryBuckets.Integrations.CsvIntegration.Tests
{
    [TestClass()]
    public class FileIntegrationTests
    {
        [TestMethod()]
        public void Reads_stories_from_the_file()
        {
            //Arrange
            var filereader = new FileREaderMock();
            var integration = new FileIntegration(filereader.Object);

            //Act
            _ = integration.FetchAsync().Result;

            //Assert
            filereader.Verify(mock => mock.ParseAsync(), Times.Once);
        }

        [TestMethod()]
        public void Returns_the_stories_read_from_file()
        {
            //Arrange
            var itemsToReturn = new[]
            {
                new FlattenedHierarchyItem(),
                new FlattenedHierarchyItem(),
                new FlattenedHierarchyItem(),
                new FlattenedHierarchyItem()
            };
            var filereader = new FileREaderMock(itemsToReturn);
            var integration = new FileIntegration(filereader.Object);

            //Act
            var result = integration.FetchAsync().Result;

            //Assert
            Assert.AreEqual(itemsToReturn.Count(), result.Count());
            foreach (var item in result)
            {
                Assert.IsNotNull(item);
            }
        }

        [TestMethod()]
        public void Maps_Id()
        {
            //Arrange
            var id = 42;
            var itemsToReturn = new[]
            {
                new FlattenedHierarchyItem{
                    Id = id
                }
            };
            var filereader = new FileREaderMock(itemsToReturn);
            var integration = new FileIntegration(filereader.Object);

            //Act
            var result = integration.FetchAsync().Result;

            //Assert
            Assert.AreEqual(id, result.Single().Id);
        }

        [TestMethod()]
        public void Maps_Title()
        {
            //Arrange
            var title = "Foobar deluxe";
            var itemsToReturn = new[]
            {
                new FlattenedHierarchyItem{
                    Title = title
                }
            };
            var filereader = new FileREaderMock(itemsToReturn);
            var integration = new FileIntegration(filereader.Object);

            //Act
            var result = integration.FetchAsync().Result;

            //Assert
            Assert.AreEqual(title, result.Single().Title);
        }

        [TestMethod()]
        public void Do_not_include_parents_as_separate_items()
        {
            //Arrange
            var id = 42;
            var parentId = 2718;
            var itemsToReturn = new[]
            {
                new FlattenedHierarchyItem{
                    Id = id,
                    ParentId = parentId
                },
                new FlattenedHierarchyItem
                {
                    Id = parentId
                }
            };
            var filereader = new FileREaderMock(itemsToReturn);
            var integration = new FileIntegration(filereader.Object);

            //Act
            var result = integration.FetchAsync().Result;

            //Assert
            Assert.AreEqual(id, result.Single().Id);
        }

        private class FileREaderMock : Mock<IFileReader>
        {
            public FileREaderMock(IEnumerable<FlattenedHierarchyItem> itemsToReturn = null) : base()
            {
                if (itemsToReturn == null)
                    itemsToReturn = Enumerable.Empty<FlattenedHierarchyItem>();

                Setup(fake => fake.ParseAsync()).Returns(MakeAsync(itemsToReturn));
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
}