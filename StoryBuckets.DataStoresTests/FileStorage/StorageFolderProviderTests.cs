using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StoryBuckets.Options;
using System;
using System.Collections.Generic;
using System.Text;
using Utils;

namespace StoryBuckets.DataStores.FileStorage.Tests
{
    [TestClass()]
    public class StorageFolderProviderTests
    {
        [TestMethod()]
        public void Gets_base_path_from_pathprovider()
        {
            //Arrange
            var basePath = @"basepath\foo\bar";
            var pathprovider = new Mock<IStoragePathProvider>();
            pathprovider
                .Setup(fake => fake.GetStorageBasePath())
                .Returns(basePath);

            var directoryIO = new Mock<IFilesystemIo>();

            var folderprovider = new StorageFolderProvider(pathprovider.Object, directoryIO.Object);

            //Act
            _ = folderprovider.GetStorageFolder<object>("foobar");

            //Assert
            pathprovider.Verify(mock => mock.GetStorageBasePath(), Times.Once);
        }

        [TestMethod()]
        public void Combines_basepath_and_foldername_and_creates_the_directory()
        {
            //Arrange
            var basePath = @"basepath\foo\bar\";
            var foldername = "foobar";
            
            var pathprovider = new Mock<IStoragePathProvider>();
            pathprovider
                .Setup(fake => fake.GetStorageBasePath())
                .Returns(basePath);

            var createdPath = "";
            var directoryIO = new Mock<IFilesystemIo>();
            directoryIO
                .Setup(mock => mock.CreateDirectory(It.IsAny<string>()))
                .Callback<string>((path) => createdPath = path);

            var folderprovider = new StorageFolderProvider(pathprovider.Object, directoryIO.Object);

            //Act
            _ = folderprovider.GetStorageFolder<object>(foldername);

            //Assert
            directoryIO.Verify(mock => mock.CreateDirectory(It.IsAny<string>()), Times.Once);
            Assert.AreEqual($"{basePath}{foldername}", createdPath);
        }

        [TestMethod()]
        public void Adds_backslash_to_path_if_needed()
        {
            //Arrange
            var foldername = "foobar";
            var basePath = @"basepath\foo\bar";

            var pathprovider = new Mock<IStoragePathProvider>();
            pathprovider
                .Setup(fake => fake.GetStorageBasePath())
                .Returns(basePath);

            var createdPath = "";
            var directoryIO = new Mock<IFilesystemIo>();
            directoryIO
                .Setup(mock => mock.CreateDirectory(It.IsAny<string>()))
                .Callback<string>((path) => createdPath = path);

            var folderprovider = new StorageFolderProvider(pathprovider.Object, directoryIO.Object);

            //Act
            var storageFolder = folderprovider.GetStorageFolder<object>(foldername);

            //Assert
            Assert.AreEqual(@$"{basePath}\{foldername}", createdPath);
        }
    }
}