using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Utils;

namespace StoryBuckets.Options
{
    public class AppDataPathProvider : IStoragePathProvider, IIntegrationPathProvider
    {
        private readonly IFilesystemIo _filesystem;
        private string _basepath;
        private string _integrationpath;
        private string _storagepath;
        private string integrationFileName = "stories";

        public AppDataPathProvider(IFilesystemIo filesystem)
        {
            _filesystem = filesystem;
        }

        public string IntegrationFileName { 
            get => integrationFileName;
            set => integrationFileName = value ?? throw new ArgumentNullException(nameof(IntegrationFileName)); 
        }

        public string Basepath
        {
            get
            {
                if (_basepath == null)
                {
                    _basepath = GetBasePath();
                }
                return _basepath;
            }
        }

        public string GetIntegrationPath()
        {
            if (_integrationpath == null)
            {
                _integrationpath = Path.Combine(Basepath, "integrationfiles");
                _filesystem.CreateDirectory(_integrationpath);
            }
            return Path.Combine(_integrationpath, integrationFileName);
        }

        public string GetPathToIntegrationFile()
        {
            return GetIntegrationPath();
        }

        public string GetStorageBasePath()
        {
            if (_storagepath == null)
            {
                _storagepath = Path.Combine(Basepath, "storage");
                _filesystem.CreateDirectory(_storagepath);
            }
            return _storagepath;
        }

        private string GetBasePath()
        {

            string appdataPath = _filesystem.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            if (string.IsNullOrEmpty(appdataPath))
                throw new InvalidOperationException("No Appdata folder was found!");

            return Path.Combine(appdataPath, "bkhedblom", "StoryBuckets");
        }
    }
}
