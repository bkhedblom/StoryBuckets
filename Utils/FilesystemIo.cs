using System.Collections.Generic;
using System.IO;

namespace Utils
{
    public class FilesystemIo : IFilesystemIo
    {
        public void CreateDirectory(string path)
            => Directory.CreateDirectory(path);

        public string GetTempPath()
             => Path.GetTempPath();

        public IEnumerable<string> EnumerateFiles(string path)
            => Directory.EnumerateFiles(path);

        public FileStream Open(string filename, FileMode mode, FileAccess access)
            => File.Open(filename, mode, access);
    }
}
