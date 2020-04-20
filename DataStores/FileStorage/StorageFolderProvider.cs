using StoryBuckets.DataStores.FileStore;
using StoryBuckets.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Utils;

namespace StoryBuckets.DataStores.FileStorage
{
    public class StorageFolderProvider : IStorageFolderProvider
    {
        private readonly IStoragePathProvider _pathProvider;
        private readonly IFilesystemIo _directoryIO;

        public StorageFolderProvider(IStoragePathProvider pathProvider, IFilesystemIo directoryIO)
        {
            _pathProvider = pathProvider;
            _directoryIO = directoryIO;
        }
        public IStorageFolder<T> GetStorageFolder<T>(string foldername)
        {
            var basePath = _pathProvider.GetStorageBasePath();
            var folderPath = Path.Combine(basePath, foldername);
            return new StorageFolder<T>(folderPath, _directoryIO);
        }
    }
}
