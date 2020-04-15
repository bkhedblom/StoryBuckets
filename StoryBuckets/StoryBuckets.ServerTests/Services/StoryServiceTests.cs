using Microsoft.VisualStudio.TestTools.UnitTesting;
using StoryBuckets.Server.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoryBuckets.Server.Services.Tests
{
    [TestClass()]
    public class StoryServiceTests
    {
        [TestMethod()]
        public void Get_returns_some_stories()
        {
            //Arrange
            var service = new StoryService();

            //Act
            var result = service.GetAsync().Result;

            //Assert
            Assert.IsNotNull(result);
        }
    }
}