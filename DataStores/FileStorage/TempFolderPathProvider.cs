using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Utils;

namespace StoryBuckets.DataStores.FileStorage
{
    public class TempFolderPathProvider : IPathProvider
    {
        private readonly IFilesystemIo _filesystem;

        public TempFolderPathProvider(IFilesystemIo filesystem)
        {
            _filesystem = filesystem;
        }
        public string GetStorageBasePath()
        {
            var tempFolder = _filesystem.GetTempPath();
            var applicationStoragePath = Path.Combine(tempFolder, "StoryBuckets", "Storage");
            _filesystem.CreateDirectory(applicationStoragePath);
            return applicationStoragePath;
        }
    }
}
