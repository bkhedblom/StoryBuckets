﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Utils;

namespace StoryBuckets.DataStores.FileStorage
{
    internal class StorageFolder<T> : IStorageFolder<T>
    {
        private readonly string _path;
        private readonly IFilesystemIo _filesystem;
        private static readonly SemaphoreSlim _fileProcessLock = new SemaphoreSlim(1);

        public StorageFolder(string folderpath, IFilesystemIo filesystem)
        {
            _path = folderpath;
            _filesystem = filesystem;
            filesystem.CreateDirectory(folderpath);
        }

        public async IAsyncEnumerable<T> GetStoredItemsAsync()
        {
            foreach (var filename in _filesystem.EnumerateFiles(_path))
            {
                var fullPath = Path.Combine(_path, filename);
                await _fileProcessLock.WaitAsync();
                try
                {
                    using (var fileStream = _filesystem.Open(fullPath, FileMode.Open, FileAccess.Read))
                    {
                        yield return await JsonSerializer.DeserializeAsync<T>(fileStream);
                    }
                }
                finally
                {
                    _fileProcessLock.Release();
                }
            }
        }

        public async Task CreateFileForItemAsync(T item, string filename)
        {
            var fullPath = Path.Combine(_path, filename);
            await _fileProcessLock.WaitAsync();
            try
            {
                using (var fileStream = _filesystem.Open(fullPath, FileMode.CreateNew, FileAccess.Write))
                {
                    await JsonSerializer.SerializeAsync(fileStream, item);
                }
            }
            finally
            {
                _fileProcessLock.Release();
            }
        }


        public async Task ReplaceFileWithItemAsync(string filename, T item)
        {
            var fullPath = Path.Combine(_path, filename);
            await _fileProcessLock.WaitAsync();
            try
            {
                using (var fileStream = _filesystem.Open(fullPath, FileMode.Truncate, FileAccess.Write))
                {
                    await JsonSerializer.SerializeAsync(fileStream, item);
                }
            }
            finally
            {
                _fileProcessLock.Release();
            }
        }
    }
}
