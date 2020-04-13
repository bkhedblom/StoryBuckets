using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StoryBuckets.Client.Models;
using StoryBuckets.Client.ServerCommunication;
using StoryBuckets.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoryBuckets.Client.Models.Tests
{
    [TestClass()]
    public class StorylistTests
    {
        //[TestMethod()]
        //public void Should_fetch_stories_from_server_on_creation()
        //{
        //    //Arrange
        //    var reader = new Mock<IDataReader<IStory>>();

        //    //Act
        //    new Storylist(reader.Object);

        //    //Assert
        //    reader.Verify(mock => mock.Read(), Times.Once);
        //}

        //Vi måste istället hämta från servern när vi försöker komma åt data första gången
    }
}