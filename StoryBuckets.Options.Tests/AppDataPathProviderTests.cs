using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StoryBuckets.Options;
using System;
using System.Collections.Generic;
using System.Text;
using Utils;

namespace StoryBuckets.Options.Tests
{
    [TestClass()]
    public class AppDataPathProviderTests
    {
        private const string AppDataPath = @"c:\users\foobar\AppData\Local";
        private static string appDataForStoryBuckets = @$"{AppDataPath}\bkhedblom\StoryBuckets";
        private static string fullPathForStorage = @$"{appDataForStoryBuckets}\storage";
        private static string fullPathForIntegration = @$"{appDataForStoryBuckets}\integrationfiles";

        [TestMethod()]
        public void IntegrationPath_gets_and_uses_the_AppData_path()
        {
            //Arrange
            var filesystem = GetFilesystemMock();
            var pathprovider = new AppDataPathProvider(filesystem.Object);

            //Act
            var returnedPath = pathprovider.GetPathToIntegrationFile();

            //Assert
            filesystem.Verify(mock => mock.GetFolderPath(Environment.SpecialFolder.ApplicationData), Times.Once);
            Assert.IsTrue(returnedPath.StartsWith(AppDataPath));
        }

        [TestMethod()]
        public void GetStorageBasePath_gets_and_uses_the_AppData_path()
        {
            //Arrange
            var filesystem = GetFilesystemMock();
            var pathprovider = new AppDataPathProvider(filesystem.Object);

            //Act
            var returnedPath = pathprovider.GetStorageBasePath();

            //Assert
            filesystem.Verify(mock => mock.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            Assert.IsTrue(returnedPath.StartsWith(AppDataPath));
        }

        [TestMethod()]
        public void Adds_bkhedblom_StoryBuckets_to_the_appdata_path()
        {
            //Arrange
            var filesystem = GetFilesystemMock();
            var pathprovider = new AppDataPathProvider(filesystem.Object);

            //Act
            var returnedStoragePath = pathprovider.GetStorageBasePath();
            var returnedIntegrationPath = pathprovider.GetPathToIntegrationFile();

            //Assert
            Assert.IsTrue(returnedStoragePath.StartsWith(appDataForStoryBuckets));
            Assert.IsTrue(returnedIntegrationPath.StartsWith(appDataForStoryBuckets));
        }

        [TestMethod()]
        public void Throws_InvalidOperationException_if_appdata_folder_does_not_exist()
        {
            //Arrange
            var filesystem = GetFilesystemMock(appDataPath: string.Empty);
            var pathprovider = new AppDataPathProvider(filesystem.Object);

            //Act && 
            //Assert
            Assert.ThrowsException<InvalidOperationException>(() => _ = pathprovider.GetStorageBasePath());
            Assert.ThrowsException<InvalidOperationException>(() => _ = pathprovider.GetPathToIntegrationFile());
        }

        [TestMethod()]
        public void GetStorageBasePath_returns_correct_path()
        {
            Mock<IFilesystemIo> filesystem = GetFilesystemMock();
            var pathprovider = new AppDataPathProvider(filesystem.Object);

            //Act
            var returnedStoragePath = pathprovider.GetStorageBasePath();

            //Assert
            Assert.AreEqual(fullPathForStorage, returnedStoragePath);
        }

        [TestMethod()]
        public void GetIntegrationPath_returns_correct_path()
        {
            Mock<IFilesystemIo> filesystem = GetFilesystemMock();
            var pathprovider = new AppDataPathProvider(filesystem.Object);

            //Act
            var returnedIntegrationPath = pathprovider.GetPathToIntegrationFile();

            //Assert
            Assert.IsTrue(returnedIntegrationPath.StartsWith(fullPathForIntegration));
        }

        [TestMethod()]
        public void GetStorageBasePath_creates_the_directory()
        {
            //Arrange
            var filesystem = GetFilesystemMock();
            var pathprovider = new AppDataPathProvider(filesystem.Object);

            //Act
            _ = pathprovider.GetStorageBasePath();

            //Assert
            filesystem.Verify(mock => mock.CreateDirectory(fullPathForStorage), Times.Once);            
        }

        [TestMethod()]
        public void IntegrationPath_creates_the_directory()
        {
            //Arrange
            var filesystem = GetFilesystemMock();
            var pathprovider = new AppDataPathProvider(filesystem.Object);

            //Act
            _ = pathprovider.GetPathToIntegrationFile();

            //Assert
            filesystem.Verify(mock => mock.CreateDirectory(fullPathForIntegration), Times.Once);
        }

        [TestMethod()]
        public void GetIntegrationPath_includes_filename()
        {
            Mock<IFilesystemIo> filesystem = GetFilesystemMock();
            var pathprovider = new AppDataPathProvider(filesystem.Object);

            //Act
            var returnedIntegrationPath = pathprovider.GetPathToIntegrationFile();

            //Assert
            Assert.AreEqual(@$"{fullPathForIntegration}\{pathprovider.IntegrationFileName}", returnedIntegrationPath);
        }

        [TestMethod()]
        public void Throws_ArgumentNullException_when_trying_to_set_IntegrationFileName_to_null()
        {
            //Arrange
            var filesystem = GetFilesystemMock(appDataPath: string.Empty);
            var pathprovider = new AppDataPathProvider(filesystem.Object);

            //Act && 
            //Assert
            Assert.ThrowsException<ArgumentNullException>(() => pathprovider.IntegrationFileName = null);
        }

        private static Mock<IFilesystemIo> GetFilesystemMock(string appDataPath = AppDataPath)
        {
            //Arrange
            var filesystem = new Mock<IFilesystemIo>();
            filesystem
                .Setup(fake => fake.GetFolderPath(It.IsAny<Environment.SpecialFolder>()))
                .Returns(appDataPath);
            return filesystem;
        }
    }
}