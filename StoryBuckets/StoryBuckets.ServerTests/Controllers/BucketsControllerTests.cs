using Microsoft.VisualStudio.TestTools.UnitTesting;
using StoryBuckets.Server.Controllers;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoryBuckets.Server.Controllers.Tests
{
    [TestClass()]
    public class BucketsControllerTests
    {
        [TestMethod()]
        public void Get_returns_a_possibly_empty_list_of_buckets()
        {
            //Arrange
            var controller = new BucketsController();

            //Act
            var result = controller.Get().Result;

            //Assert
            Assert.IsNotNull(result);
        }
    }
}