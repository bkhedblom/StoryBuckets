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
        private readonly IFilesystemIo _filesystem;

        public StorageFolderProvider(IStoragePathProvider pathProvider, IFilesystemIo filesystemIo)
        {
            _pathProvider = pathProvider;
            _filesystem = filesystemIo;
        }
        public IStorageFolder<T> GetStorageFolder<T>(string foldername)
        {
            var basePath = _pathProvider.GetStorageBasePath();
            var folderPath = Path.Combine(basePath, foldername);
            return new StorageFolder<T>(folderPath, _filesystem);
        }
    }
}
