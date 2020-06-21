using Microsoft.VisualStudio.TestTools.UnitTesting;
using StoryBuckets.Shared;
using StoryBuckets.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoryBuckets.Shared.Tests
{
    [TestClass()]
    public class LinkedBucketsTests
    {
        [TestMethod()]
        public void ctor_Can_Be_initialised_without_collection()
        {
            //Arrange

            //Act & Assert
            _ = new LinkedBuckets();

            //Passes by not throwing exception
        }

        [TestMethod()]
        public void ctor_Can_Be_initialised_with_collection_of_IBucket()
        {
            //Arrange
            ICollection<IBucket> buckets = new[]
            {
                new Bucket{ Id = 1 }
            };

            //Act & Assert
            _ = new LinkedBuckets(buckets);

            //Passes by not throwing exception
        }

        [TestMethod()]
        public void ctor_allows_empty_collection()
        {
            //Arrange
            var buckets = new List<IBucket>();

            //Act & Assert
            _ = new LinkedBuckets(buckets);

            //Passes by not throwing exception
        }

        [TestMethod()]
        public void Enumerator_Current_starts_as_null()
        {
            //Arrange
            var buckets = new[]
            {
                new Bucket{ Id = 1 }
            };
            var testing = new LinkedBuckets(buckets);

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
                new Bucket{ Id = 1 }
            };
            var testing = new LinkedBuckets(buckets);
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
                new Bucket{ Id = 1 }
            };
            var testing = new LinkedBuckets(buckets);
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
                new Bucket{ Id = 1 }
            };
            var testing = new LinkedBuckets(buckets);
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
            var testing = new LinkedBuckets();
            var enumerator = testing.GetEnumerator();

            //Act
            _ = enumerator.MoveNext();

            //Assert: It didn't throw = success
        }

        [TestMethod()]
        public void Enumerator_Reset_throws_NotSupportedException()
        {
            //Arrange
            var testing = new LinkedBuckets();
            var enumerator = testing.GetEnumerator();

            //Act & Assert
            Assert.ThrowsException<NotSupportedException>(() => enumerator.Reset());
        }

        [TestMethod()]
        public void Enumerator_MoveNext_sets_Current_to_NextBigger_Bucket()
        {
            //Arrange
            var biggerBucket = new Bucket { Id = 42 };
            var smallerBucket = new Bucket { Id = 2, NextBiggerBucket = biggerBucket };
            var buckets = new[]
            {
                smallerBucket,
                biggerBucket
            };
            var testing = new LinkedBuckets(buckets);
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
            ICollection<IBucket> buckets = new[]
            {
                new Bucket{ Id = 1 },
                new Bucket{ Id = 2 }
            };

            //Act & Assert
            Assert.ThrowsException<InvalidOperationException>(() =>  new LinkedBuckets(buckets));
        }

        [TestMethod()]
        public void ctor_throws_InvalidOperationException_if_initialised_with_collection_where_more_than_one_Bucket_has_the_same_NextBiggerBucket()
        {
            //Arrange
            var biggerBucket = new Bucket { Id = 3 };

            ICollection<IBucket> buckets = new[]
            {
                new Bucket{ Id = 1, NextBiggerBucket = biggerBucket },
                new Bucket{ Id = 2, NextBiggerBucket = biggerBucket },
                biggerBucket
            };

            //Act & Assert
            Assert.ThrowsException<InvalidOperationException>(() => new LinkedBuckets(buckets));
        }

        [TestMethod()]
        public void ctor_throws_InvalidOperationException_if_initialised_with_collection_where_some_NextBiggerBucket_is_not_in_collection()
        {
            //Arrange
            var biggerBucket = new Bucket { Id = 3 };

            ICollection<IBucket> buckets = new[]
            {
                new Bucket{ Id = 1, NextBiggerBucket = biggerBucket },
            };

            //Act & Assert
            Assert.ThrowsException<InvalidOperationException>(() => new LinkedBuckets(buckets));
        }

        [TestMethod()]
        public void ctor_throws_InvalidOperationException_if_initialised_with_collection_with_circular_NextBiggerBucket_references()
        {
            //Arrange
            var biggerBucket = new Bucket { Id = 314 };
            var mediumBucket = new Bucket { Id = 42, NextBiggerBucket = biggerBucket };
            var circularBucket = new Bucket { Id = 8, NextBiggerBucket = mediumBucket };

            biggerBucket.NextBiggerBucket = circularBucket;

            ICollection<IBucket> buckets = new[]
            {
                circularBucket,
                mediumBucket,
                biggerBucket
            };

            //Act & Assert
            Assert.ThrowsException<InvalidOperationException>(() => new LinkedBuckets(buckets));
        }

        [TestMethod()]
        public void ctor_throws_InvalidOperationException_if_initialised_with_collection_with_more_than_one_smallest_bucket()
        {
            //Arrange
            var biggerBucket = new Bucket { Id = 314 };
            var mediumBucket = new Bucket { Id = 42, NextBiggerBucket = biggerBucket };


            ICollection<IBucket> buckets = new[]
            {
                new Bucket { Id = 8, NextBiggerBucket = mediumBucket },
                new Bucket { Id = 10, NextBiggerBucket = biggerBucket },
                mediumBucket,
                biggerBucket
            };

            //Act & Assert
            Assert.ThrowsException<InvalidOperationException>(() => new LinkedBuckets(buckets));
        }

        [TestMethod()]
        public void Enumerator_MoveNext_sets_the_first_Current_to_the_only_one_that_is_not_bigger_than_any_of_the_others()
        {
            //Arrange
            var biggerBucket = new Bucket { Id = 42 };
            var smallerBucket = new Bucket { Id = 2, NextBiggerBucket = biggerBucket };
            var buckets = new[]
            {
                biggerBucket,
                smallerBucket
            };
            var testing = new LinkedBuckets(buckets);
            var enumerator = testing.GetEnumerator();

            //Act
            _ = enumerator.MoveNext();

            //Assert
            Assert.AreEqual(smallerBucket, enumerator.Current, $"Expected Bucket with Id {smallerBucket.Id}, got Id {enumerator.Current.Id}");
        }
    }
}