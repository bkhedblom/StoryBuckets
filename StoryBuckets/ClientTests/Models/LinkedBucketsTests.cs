using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StoryBuckets.Client.ServerCommunication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StoryBuckets.Client.Models.Tests
{
    [TestClass()]
    public class LinkedBucketsTests
    {
        [TestMethod()]
        public void ctor_Can_Be_initialised_with_ReadOnlyCollection_of_SyncableBucket()
        {
            //Arrange
            IReadOnlyCollection<SyncableBucket> buckets = new[]
            {
                new SyncableBucket{ Id = 1 }
            };
            var dataCreator = new Mock<IDataCreator<SyncableBucket>>();

            //Act & Assert
            _ = new LinkedSyncableBuckets(dataCreator.Object, buckets);

            //Passes by not throwing exception
        }

        [TestMethod()]
        public void ctor_allows_empty_collection()
        {
            //Arrange
            var buckets = new List<SyncableBucket>();
            var dataCreator = new Mock<IDataCreator<SyncableBucket>>();

            //Act & Assert
            _ = new LinkedSyncableBuckets(dataCreator.Object, buckets);

            //Passes by not throwing exception
        }

        [TestMethod()]
        public void Enumerator_Current_starts_as_null()
        {
            //Arrange
            var buckets = new[]
            {
                new SyncableBucket{ Id = 1 }
            };
            var dataCreator = new Mock<IDataCreator<SyncableBucket>>();
            var testing = new LinkedSyncableBuckets(dataCreator.Object, buckets);

            //Act
            var enumerator = testing.GetEnumerator();

            //Assert
            Assert.IsNull(enumerator.Current);
        }


        [TestMethod()]
        public void Enumerator_MoveNext_sets_Current()
        {
            //Arrange
            var buckets = new[]
            {
                new SyncableBucket{ Id = 1 }
            };
            var dataCreator = new Mock<IDataCreator<SyncableBucket>>();
            var testing = new LinkedSyncableBuckets(dataCreator.Object, buckets);
            var enumerator = testing.GetEnumerator();

            //Act
            _ = enumerator.MoveNext();

            //Assert
            Assert.IsNotNull(enumerator.Current);
        }

        [TestMethod()]
        public void Enumerator_MoveNext_returns_true_until_the_last_element_is_passed()
        {
            //Arrange
            var buckets = new[]
            {
                new SyncableBucket{ Id = 1 }
            };
            var dataCreator = new Mock<IDataCreator<SyncableBucket>>();
            var testing = new LinkedSyncableBuckets(dataCreator.Object, buckets);
            var enumerator = testing.GetEnumerator();

            //Act
            var firstMoveNext = enumerator.MoveNext();
            var secondMoveNext = enumerator.MoveNext();

            //Assert
            Assert.IsTrue(firstMoveNext);
            Assert.IsFalse(secondMoveNext);
        }

        [TestMethod()]
        public void Enumerator_MoveNext_sets_current_null_when_the_last_element_is_passed()
        {
            //Arrange
            var buckets = new[]
            {
                new SyncableBucket{ Id = 1 }
            };
            var dataCreator = new Mock<IDataCreator<SyncableBucket>>();
            var testing = new LinkedSyncableBuckets(dataCreator.Object, buckets);
            var enumerator = testing.GetEnumerator();

            //Act
            _ = enumerator.MoveNext();
            _ = enumerator.MoveNext();

            //Assert
            Assert.IsNull(enumerator.Current);
        }

        [TestMethod()]
        public void Enumerator_MoveNext_works_if_created_without_collection()
        {
            //Arrange
            var dataCreator = new Mock<IDataCreator<SyncableBucket>>();
            var testing = new LinkedSyncableBuckets(dataCreator.Object, new List<SyncableBucket>());
            var enumerator = testing.GetEnumerator();

            //Act
            _ = enumerator.MoveNext();

            //Assert: It didn't throw = success
        }

        [TestMethod()]
        public void Enumerator_Reset_throws_NotSupportedException()
        {
            //Arrange
            var dataCreator = new Mock<IDataCreator<SyncableBucket>>();
            var testing = new LinkedSyncableBuckets(dataCreator.Object, new List<SyncableBucket>());
            var enumerator = testing.GetEnumerator();

            //Act & Assert
            Assert.ThrowsException<NotSupportedException>(() => enumerator.Reset());
        }

        [TestMethod()]
        public void Enumerator_MoveNext_sets_Current_to_NextBigger_Bucket()
        {
            //Arrange
            var biggerBucket = new SyncableBucket { Id = 42 };
            var smallerBucket = new SyncableBucket { Id = 2, NextBiggerBucket = biggerBucket };
            var buckets = new[]
            {
                smallerBucket,
                biggerBucket
            };
            var dataCreator = new Mock<IDataCreator<SyncableBucket>>();
            var testing = new LinkedSyncableBuckets(dataCreator.Object, buckets);
            var enumerator = testing.GetEnumerator();

            //Act
            _ = enumerator.MoveNext();
            Assert.AreEqual(smallerBucket, enumerator.Current, "Test not valid. If the smaller bucket is not the first one, how are we supposed to test what follows after it?");
            _ = enumerator.MoveNext();

            //Assert
            Assert.AreEqual(biggerBucket, enumerator.Current, $"Expected Bucket with Id {biggerBucket.Id}, got Id {enumerator.Current.Id}");
        }

        [TestMethod()]
        public void ctor_throws_InvalidOperationException_if_initialised_with_collection_containing_more_than_one_Bucket_without_a_NextBiggerBucket()
        {
            //Arrange
            var buckets = new[]
            {
                new SyncableBucket{ Id = 1 },
                new SyncableBucket{ Id = 2 }
            };
            var dataCreator = new Mock<IDataCreator<SyncableBucket>>();

            //Act & Assert
            Assert.ThrowsException<InvalidOperationException>(() => new LinkedSyncableBuckets(dataCreator.Object, buckets));
        }

        [TestMethod()]
        public void ctor_throws_InvalidOperationException_if_initialised_with_collection_where_more_than_one_Bucket_has_the_same_NextBiggerBucket()
        {
            //Arrange
            var biggerBucket = new SyncableBucket { Id = 3 };

            var buckets = new[]
            {
                new SyncableBucket{ Id = 1, NextBiggerBucket = biggerBucket },
                new SyncableBucket{ Id = 2, NextBiggerBucket = biggerBucket },
                biggerBucket
            };
            var dataCreator = new Mock<IDataCreator<SyncableBucket>>();

            //Act & Assert
            Assert.ThrowsException<InvalidOperationException>(() => new LinkedSyncableBuckets(dataCreator.Object, buckets));
        }

        [TestMethod()]
        public void ctor_throws_InvalidOperationException_if_initialised_with_collection_where_some_NextBiggerBucket_is_not_in_collection()
        {
            //Arrange
            var biggerBucket = new SyncableBucket { Id = 3 };

            var buckets = new[]
            {
                new SyncableBucket{ Id = 1, NextBiggerBucket = biggerBucket },
            };
            var dataCreator = new Mock<IDataCreator<SyncableBucket>>();

            //Act & Assert
            Assert.ThrowsException<InvalidOperationException>(() => new LinkedSyncableBuckets(dataCreator.Object, buckets));
        }

        [TestMethod()]
        public void ctor_throws_InvalidOperationException_if_initialised_with_collection_with_circular_NextBiggerBucket_references()
        {
            //Arrange
            var biggerBucket = new SyncableBucket { Id = 314 };
            var mediumBucket = new SyncableBucket { Id = 42, NextBiggerBucket = biggerBucket };
            var circularBucket = new SyncableBucket { Id = 8, NextBiggerBucket = mediumBucket };

            biggerBucket.NextBiggerBucket = circularBucket;

            var buckets = new[]
            {
                circularBucket,
                mediumBucket,
                biggerBucket
            };
            var dataCreator = new Mock<IDataCreator<SyncableBucket>>();

            //Act & Assert
            Assert.ThrowsException<InvalidOperationException>(() => new LinkedSyncableBuckets(dataCreator.Object, buckets));
        }

        [TestMethod()]
        public void ctor_throws_InvalidOperationException_if_initialised_with_collection_with_more_than_one_smallest_bucket()
        {
            //Arrange
            var biggerBucket = new SyncableBucket { Id = 314 };
            var mediumBucket = new SyncableBucket { Id = 42, NextBiggerBucket = biggerBucket };


            var buckets = new[]
            {
                new SyncableBucket { Id = 8, NextBiggerBucket = mediumBucket },
                new SyncableBucket { Id = 10, NextBiggerBucket = biggerBucket },
                mediumBucket,
                biggerBucket
            };
            var dataCreator = new Mock<IDataCreator<SyncableBucket>>();

            //Act & Assert
            Assert.ThrowsException<InvalidOperationException>(() => new LinkedSyncableBuckets(dataCreator.Object, buckets));
        }

        [TestMethod()]
        public void Enumerator_MoveNext_sets_the_first_Current_to_the_only_one_that_is_not_bigger_than_any_of_the_others()
        {
            //Arrange
            var biggerBucket = new SyncableBucket { Id = 42 };
            var smallerBucket = new SyncableBucket { Id = 2, NextBiggerBucket = biggerBucket };
            var buckets = new[]
            {
                biggerBucket,
                smallerBucket
            };
            var dataCreator = new Mock<IDataCreator<SyncableBucket>>();
            var testing = new LinkedSyncableBuckets(dataCreator.Object, buckets);
            var enumerator = testing.GetEnumerator();

            //Act
            _ = enumerator.MoveNext();

            //Assert
            Assert.AreEqual(smallerBucket, enumerator.Current, $"Expected Bucket with Id {smallerBucket.Id}, got Id {enumerator.Current.Id}");
        }
    }
}