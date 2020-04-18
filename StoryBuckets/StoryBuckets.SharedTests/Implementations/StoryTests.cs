using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoryBuckets.Shared.Implementations.Tests
{
    [TestClass()]
    public class StoryTests
    {
        [TestMethod()]
        public void String_representation_contains_id()
        {
            //Arrange
            var id = 31415;
            var story = new Story { 
                Id = id
            };

            //Act
            var stringRepresentation = story.ToString();

            //Assert
            var stringContainsId = stringRepresentation.Contains(id.ToString());
            Assert.IsTrue(stringContainsId);
        }

        [TestMethod()]
        public void String_representation_contains_title()
        {
            //Arrange
            var title = "foobar";
            var story = new Story
            {
                Title = title
            };

            //Act
            var stringRepresentation = story.ToString();

            //Assert
            var stringContainsId = stringRepresentation.Contains(title);
            Assert.IsTrue(stringContainsId);
        }
    }
}