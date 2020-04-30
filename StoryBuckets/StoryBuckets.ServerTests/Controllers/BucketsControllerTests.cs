using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StoryBuckets.Server.Controllers;
using StoryBuckets.Services;
using StoryBuckets.Shared;
using StoryBuckets.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryBuckets.Server.Controllers.Tests
{
    [TestClass()]
    public class BucketsControllerTests
    {
        [TestMethod()]
        public void Get_returns_a_possibly_empty_list_of_buckets()
        {
            //Arrange
            var service = new Mock<IBucketService>();
            service
                .Setup(fake => fake.GetAllAsync())
                .ReturnsAsync(Enumerable.Empty<Bucket>());


            var controller = new BucketsController(service.Object);

            //Act
            var result = controller.Get().Result;

            //Assert
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void Get_returns_data_From_Service_verbatim()
        {
            //Arrange
            var buckets = new[]
                {
                    new Bucket(),
                    new Bucket(),
                    new Bucket()
                };
            var service = new Mock<IBucketService>();
            service
                .Setup(fake => fake.GetAllAsync())
                .ReturnsAsync(buckets);

            var controller = new BucketsController(service.Object);

            //Act
            var result = controller.Get().Result;

            //Assert
            service.Verify(mock => mock.GetAllAsync(), Times.Once);
            Assert.AreEqual(buckets.Count(), result.Count());
            Assert.AreEqual(buckets.First(), result.First());
        }

        [TestMethod()]
        public async Task Post_adds_the_bucket_and_returns_it_updated()
        {
            //Arrange
            var service = new Mock<IBucketService>();
            var controller = new BucketsController(service.Object);
                        
            var bucket = new Bucket();

            //Act
            var returned = await controller.Post(bucket);

            //Assert
            service.Verify(mock => mock.Add(bucket), Times.Once);
            Assert.AreEqual(bucket, returned);
        }
    }
}