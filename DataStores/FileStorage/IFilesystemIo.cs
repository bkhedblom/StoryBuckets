﻿using System.Collections.Generic;
using System.IO;

namespace StoryBuckets.DataStores.FileStorage
{
    public interface IFilesystemIo
    {
        void CreateDirectory(string path);
        string GetTempPath();
        IEnumerable<string> EnumerateFiles(string path);
        FileStream Open(string filename, FileMode mode, FileAccess access);
    }
}