using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StoryBuckets.DataStores.FileStorage;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoryBuckets.DataStores.FileStorage.Tests
{
    [TestClass()]
    public class TempFolderPathProviderTests
    {
        [TestMethod()]
        public void Gets_temp_folder()
        {
            //Arrange
            var filesystem = new Mock<IFilesystemIo>();
            filesystem
                .Setup(fake => fake.GetTempPath())
                .Returns("foobar");

            var pathprovider = new TempFolderPathProvider(filesystem.Object);

            //Act
            _ = pathprovider.GetStorageBasePath();

            //Assert
            filesystem.Verify(mock => mock.GetTempPath(), Times.Once);
        }

        [TestMethod()]
        public void Combines_temp_folder_with_StoryBuckets_Storage()
        {
            //Arrange
            var tempPath = @"C:\users\foobar\temp";
            var filesystem = new Mock<IFilesystemIo>();
            filesystem
                .Setup(fake => fake.GetTempPath())
                .Returns(@"C:\users\foobar\temp");

            var pathprovider = new TempFolderPathProvider(filesystem.Object);

            //Act
            var result = pathprovider.GetStorageBasePath();

            //Assert
            Assert.AreEqual($@"{tempPath}\StoryBuckets\Storage", result);
        }

        [TestMethod()]
        public void Creates_directory()
        {
            //Arrange
            var filesystem = new Mock<IFilesystemIo>();
            filesystem
                .Setup(fake => fake.GetTempPath())
                .Returns("foobar");

            var pathprovider = new TempFolderPathProvider(filesystem.Object);

            //Act
            _ = pathprovider.GetStorageBasePath();

            //Assert
            filesystem.Verify(mock => mock.CreateDirectory(It.IsAny<string>()), Times.Once);
        }
    }
}